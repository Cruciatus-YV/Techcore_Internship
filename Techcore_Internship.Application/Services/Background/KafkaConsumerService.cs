using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Techcore_Internship.Application.Services.Background;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("Kafka Consumer Service stopped");
    }
}
