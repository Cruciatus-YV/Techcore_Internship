using System.Text.Json;

namespace Techcore_Internship.Gateway.Aggregators;

public class BookDetailsAggregator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BookDetailsAggregator> _logger;
    private readonly bool _isRunningInDocker;

    public BookDetailsAggregator(
        IHttpClientFactory httpClientFactory,
        ILogger<BookDetailsAggregator> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ||
                           Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "True";
    }

    private string GetBaseUrl()
    {
        return _isRunningInDocker ? "http://webapi:5001" : "http://localhost:5001";
    }

    public async Task<object?> AggregateBookDetailsAsync(string bookId)
    {
        try
        {
            var baseUrl = GetBaseUrl();
            var httpClient = _httpClientFactory.CreateClient();

            var bookTask = httpClient.GetAsync($"{baseUrl}/api/Books/{bookId}");
            var reviewsTask = httpClient.GetAsync($"{baseUrl}/api/ProductReviews/product/{bookId}");

            await Task.WhenAll(bookTask, reviewsTask);

            var bookResponse = await bookTask;
            var reviewsResponse = await reviewsTask;

            if (!bookResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var bookContent = await bookResponse.Content.ReadAsStringAsync();
            var reviewsContent = reviewsResponse.IsSuccessStatusCode
                ? await reviewsResponse.Content.ReadAsStringAsync()
                : "[]";

            var bookData = JsonDocument.Parse(bookContent).RootElement;
            var reviewData = JsonDocument.Parse(reviewsContent).RootElement;

            return new
            {
                Book = bookData,
                Reviews = reviewData,
                ReviewCount = reviewData.GetArrayLength(),
                AggregatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error aggregating book details for {bookId}");
            throw;
        }
    }
}