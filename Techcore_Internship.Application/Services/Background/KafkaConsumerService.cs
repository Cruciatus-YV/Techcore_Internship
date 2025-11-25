using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            GroupId = "analytics-group",  // Добавляем GroupId
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

                _logger.LogInformation("Received message for book {BookId}: {Message}",
                    result.Message.Key, result.Message.Value);

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

    public override void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        base.Dispose();
    }
}
