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

                _logger.LogInformation("Received message for book {BookId} from partition {Partition}: {Message}",
                    result.Message.Key,
                    result.TopicPartition.Partition.Value,
                    result.Message.Value);

                await ProcessMessageAsync(result.Message.Value);

                _consumer.StoreOffset(result);
                _logger.LogInformation("Message processed and offset stored");
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

    private async Task ProcessMessageAsync(string messageJson)
    {
        try
        {
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
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message and saving to MongoDB: {Message}", messageJson);
        }
    }

    public override void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        base.Dispose();
    }
}
