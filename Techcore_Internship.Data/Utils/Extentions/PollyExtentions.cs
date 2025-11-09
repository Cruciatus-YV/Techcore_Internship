using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Wrap;
using System.Net;
using System.Text;
using System.Text.Json;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;

namespace Techcore_Internship.Data.Utils.Extentions;

public static class PollyExtentions
{
    public static AsyncPolicyWrap<HttpResponseMessage> GetPolicyWrap()
    {
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1));

        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

        var fallbackPolicy = Policy<HttpResponseMessage>
            .Handle<BrokenCircuitException>()
            .FallbackAsync(
                fallbackAction: async (cancelationToken) =>
                {
                    var defaultAuthor = new AuthorResponse
                    {
                        Id = Guid.Empty,
                        FirstName = "Unknown",
                        LastName = "Author",
                        Books = []
                    };

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(
                            JsonSerializer.Serialize(defaultAuthor),
                            Encoding.UTF8,
                            "application/json")
                    };
                });

        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(3);

        var policyWrap = Policy.WrapAsync(fallbackPolicy, timeoutPolicy, circuitBreakerPolicy, retryPolicy);

        return policyWrap;
    }
}
