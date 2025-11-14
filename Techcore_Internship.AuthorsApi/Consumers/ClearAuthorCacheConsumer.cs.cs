using MassTransit;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;
using Techcore_Internship.Data.Cache.Interfaces;

namespace Techcore_Internship.AuthorsApi.Consumers
{
    public class ClearAuthorCacheConsumer : IConsumer<ClearAuthorCacheRequest>
    {
        private readonly IRedisCacheService _cache;
        private readonly ILogger<ClearAuthorCacheConsumer> _logger;

        public ClearAuthorCacheConsumer(IRedisCacheService cache, ILogger<ClearAuthorCacheConsumer> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ClearAuthorCacheRequest> context)
        {
            var message = context.Message;
            _logger.LogInformation("Received ClearAuthorCacheRequest at {Timestamp}", message.Timestamp);

            throw new Exception("TEST: Intentional exception for retry demonstration");

            try
            {
                await _cache.RemoveAsync("authors_all");
                await _cache.RemoveAsync("authors_all_with_books");


                _logger.LogInformation("Author cache cleared successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing author cache");
                throw;
            }
        }
    }
}
