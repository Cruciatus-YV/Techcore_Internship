using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Techcore_Internship.Contracts.Configurations;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;

namespace Techcore_Internship.Application.Services.Background;

public class AverageRatingCalculatorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<RedisSettings> _redisSettings;
    private readonly TimeSpan _initialDelay;
    private readonly TimeSpan _interval;

    public AverageRatingCalculatorService(
        IServiceProvider serviceProvider,
        IOptions<RedisSettings> redisSettings,
        TimeSpan? initialDelay = null,
        TimeSpan? interval = null)
    {
        _serviceProvider = serviceProvider;
        _redisSettings = redisSettings;
        _initialDelay = initialDelay ?? TimeSpan.FromSeconds(10);
        _interval = interval ?? TimeSpan.FromMinutes(1);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(_initialDelay, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await CalculateAndStoreAverageRatingsAsync(cancellationToken);
                Console.WriteLine("Average rating calculation completed successfully");
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AverageRatingCalculatorService: {ex.Message}");
            }

            try
            {
                await Task.Delay(_interval, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private async Task CalculateAndStoreAverageRatingsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var reviewRepository = scope.ServiceProvider.GetRequiredService<IProductReviewRepository>();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

        var reviews = await reviewRepository.GetAllAsync(cancellationToken);

        if (!reviews.Any())
        {
            Console.WriteLine("No reviews found for calculation");
            return;
        }

        var productGroups = reviews.GroupBy(x => x.ProductId).ToList();
        Console.WriteLine($"Calculating average ratings for {productGroups.Count} products");

        foreach (var productGroup in productGroups)
        {
            try
            {
                var key = $"{_redisSettings.Value.InstanceName}average_book_{productGroup.Key}_rating";
                var averageRating = Math.Round(productGroup.Average(x => x.Rating), 2);

                var serialized = JsonSerializer.Serialize(averageRating);
                await cache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_redisSettings.Value.DefaultExpirationMinutes)
                }, cancellationToken);

                Console.WriteLine($"Product {productGroup.Key}: average rating {averageRating} from {productGroup.Count()} reviews");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing product {productGroup.Key}: {ex.Message}");
            }
        }
    }
}
