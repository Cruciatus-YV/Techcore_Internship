using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "analytics-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoOffsetStore = false
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        _consumer.Subscribe("book_views");
        _logger.LogInformation("Kafka Consumer started. Subscribed to topic: book_views, GroupId: analytics-group");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(stoppingToken);

                _logger.LogInformation("Received message for book {BookId} from partition {Partition}",
                    result.Message.Key, result.TopicPartition.Partition.Value);

                bool processedSuccessfully = await ProcessMessageWithRetryAsync(result.Message.Value);

                if (processedSuccessfully)
                {
                    _consumer.StoreOffset(result);
                    _logger.LogInformation("Message processed successfully, offset stored for partition {Partition}",
                        result.TopicPartition.Partition.Value);
                }
                else
                {
                    _logger.LogError("Message processing failed after all retries. Offset NOT stored for partition {Partition}",
                        result.TopicPartition.Partition.Value);
                }
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
    }

    private async Task<bool> ProcessMessageWithRetryAsync(string messageJson, int maxRetries = 3)
    {
        int attempt = 0;

        while (attempt < maxRetries)
        {
            try
            {
                // Имитируем ошибку на первых двух попытках
                if (attempt < 2 && messageJson.Contains("Test Book"))
                {
                    _logger.LogWarning("Simulating MongoDB failure on attempt {Attempt}", attempt + 1);
                    throw new Exception("Simulated MongoDB connection failure");
                }

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

                    await analyticsRepository.CreateAsync(analytics, CancellationToken.None);
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
                    await Task.Delay(1000 * attempt);
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
