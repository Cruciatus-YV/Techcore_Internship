using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Events;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Application.Services.Background;

public class KafkaConsumerService : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly int _batchSize = 10;
    private readonly int _maxDegreeOfParallelism = 4;
    private readonly ConcurrentQueue<ConsumeResult<string, string>> _messagesBuffer = new();

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "kafka:9092",
            GroupId = "analytics-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoOffsetStore = false
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
        {
            Console.WriteLine("KafkaConsumerService: Skipping in local development");
            return;
        }

        await Task.Yield();
        _consumer.Subscribe("book_views");
        _logger.LogInformation("Kafka Consumer started. Subscribed to topic: book_views, GroupId: analytics-group");

        Task batchProcessingTask = Task.CompletedTask;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(TimeSpan.FromMilliseconds(100));

                if (result != null)
                {
                    _messagesBuffer.Enqueue(result);
                    _logger.LogInformation("Received message for book {BookId} from partition {Partition}",
                        result.Message.Key, result.TopicPartition.Partition.Value);
                }

                if (_messagesBuffer.Count >= _batchSize && batchProcessingTask.IsCompleted)
                {
                    var messagesToProcess = new List<ConsumeResult<string, string>>();
                    while (_messagesBuffer.TryDequeue(out var message) && messagesToProcess.Count < _batchSize)
                    {
                        messagesToProcess.Add(message);
                    }

                    if (messagesToProcess.Any())
                    {
                        batchProcessingTask = ProcessMessageBatchAsync(messagesToProcess, stoppingToken);
                    }
                }

                await Task.Delay(10, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumer operation cancelled");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming message from Kafka");
                await Task.Delay(1000, stoppingToken);
            }
        }

        if (!_messagesBuffer.IsEmpty)
        {
            var remainingMessages = _messagesBuffer.ToList();
            await ProcessMessageBatchAsync(remainingMessages, stoppingToken);
        }

        await batchProcessingTask;
    }

    private async Task ProcessMessageBatchAsync(List<ConsumeResult<string, string>> messages, CancellationToken stoppingToken)
    {
        try
        {
            await Parallel.ForEachAsync(messages,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = _maxDegreeOfParallelism,
                    CancellationToken = stoppingToken
                },
                async (message, ct) =>
                {
                    bool processedSuccessfully = await ProcessMessageWithRetryAsync(message.Message.Value, ct);

                    if (processedSuccessfully)
                    {
                        _consumer.StoreOffset(message);
                        _logger.LogInformation("Message processed successfully, offset stored for partition {Partition}",
                            message.TopicPartition.Partition.Value);
                    }
                    else
                    {
                        _logger.LogError("Message processing failed after all retries. Offset NOT stored for partition {Partition}",
                            message.TopicPartition.Partition.Value);
                    }
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message batch");
        }
    }

    private async Task<bool> ProcessMessageWithRetryAsync(string messageJson, CancellationToken cancellationToken, int maxRetries = 3)
    {
        int attempt = 0;

        while (attempt < maxRetries)
        {
            try
            {
                //// Имитируем ошибку на первых двух попытках
                //if (attempt < 2 && messageJson.Contains("Test Book"))
                //{
                //    _logger.LogWarning("Simulating MongoDB failure on attempt {Attempt}", attempt + 1);
                //    throw new Exception("Simulated MongoDB connection failure");
                //}

                using var scope = _serviceProvider.CreateScope();
                var analyticsRepository = scope.ServiceProvider.GetRequiredService<IBookViewAnalyticsRepository>();

                var bookViewEvent = JsonSerializer.Deserialize<BookViewEvent>(messageJson);
                if (bookViewEvent != null)
                {
                    var analytics = new BookViewAnalyticsEntity
                    {
                        BookId = bookViewEvent.BookId,
                        BookTitle = bookViewEvent.BookTitle,
                        ViewDate = bookViewEvent.ViewDate,
                        EventType = bookViewEvent.EventType
                    };

                    await analyticsRepository.CreateAsync(analytics, cancellationToken);
                    _logger.LogInformation("Book view analytics saved to MongoDB for book: {BookTitle}", bookViewEvent.BookTitle);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                attempt++;
                _logger.LogWarning(ex, "Attempt {Attempt} failed to process message. Retrying...", attempt);

                if (attempt < maxRetries)
                {
                    await Task.Delay(1000 * attempt, cancellationToken);
                }
                else
                {
                    _logger.LogError(ex, "All {MaxRetries} attempts failed to process message", maxRetries);
                    return false;
                }
            }
        }

        return false;
    }

    public override void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        base.Dispose();
    }
}