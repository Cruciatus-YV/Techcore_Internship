using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Techcore_Internship.Contracts;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;

namespace Techcore_Internship.Application.Services.Background;

public class AverageRatingCalculatorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<RedisSettings> _redisSettings;

    // ТОЛЬКО Singleton зависимости в конструкторе!
    public AverageRatingCalculatorService(
        IServiceProvider serviceProvider,
        IOptions<RedisSettings> redisSettings)
    {
        _serviceProvider = serviceProvider;
        _redisSettings = redisSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await CalculateAndStoreAverageRatingsAsync(cancellationToken);
                Console.WriteLine("Average rating calculation completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AverageRatingCalculatorService: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }

    private async Task CalculateAndStoreAverageRatingsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var reviewRepository = scope.ServiceProvider.GetRequiredService<IProductReviewRepository>();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

        var reviews = await reviewRepository.GetListByPredicateAsync(x => true, cancellationToken);

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
                var reviewCount = productGroup.Count();

                var serialized = JsonSerializer.Serialize(averageRating);
                await cache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_redisSettings.Value.DefaultExpirationMinutes)
                }, cancellationToken);

                Console.WriteLine($"Product {productGroup.Key}: {averageRating} from {reviewCount} reviews");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing product {productGroup.Key}: {ex.Message}");
            }
        }

        Console.WriteLine($"Completed average rating calculation for {productGroups.Count} products");
    }
}