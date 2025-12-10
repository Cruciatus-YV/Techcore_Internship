using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Text;
using System.Text.Json;

namespace Techcore_Internship.Gateway.Aggregators;

public class BookDetailsAggregator : IDefinedAggregator
{
    public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
    {
        var bookContent = await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync();
        var reviewContent = await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync();

        var bookData = JsonDocument.Parse(bookContent).RootElement;
        var reviewData = JsonDocument.Parse(reviewContent).RootElement;

        var result = new
        {
            Book = bookData,
            Reviews = reviewData,
            ReviewCount = reviewData.GetArrayLength(),
            AggregatedAt = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(result);

        return new DownstreamResponse(
            new StringContent(json, Encoding.UTF8, "application/json"),
            System.Net.HttpStatusCode.OK,
            new List<KeyValuePair<string, IEnumerable<string>>>(),
            "OK");
    }
}