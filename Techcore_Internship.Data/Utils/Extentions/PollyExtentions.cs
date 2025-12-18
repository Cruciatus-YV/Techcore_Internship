using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Polly.Simmy;
using Polly.Simmy.Fault;

namespace Techcore_Internship.Data.Utils.Extentions;

public static class PollyExtensions
{
    public static ResiliencePipeline<HttpResponseMessage> GetResiliencePipeline()
    {
        return new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                Name = "http_retry",
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Constant,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r => (int)r.StatusCode >= 500)
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                Name = "http_circuit_breaker",
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(10),
                MinimumThroughput = 5,
                BreakDuration = TimeSpan.FromSeconds(30),
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r => (int)r.StatusCode >= 500)
            })
            .AddTimeout(TimeSpan.FromSeconds(30))
            .AddChaosFault(new ChaosFaultStrategyOptions
            {
                Name = "chaos_fault",
                Enabled = true,
                InjectionRate = 0.05,
                FaultGenerator = _ =>
                    new ValueTask<Exception?>(
                        new HttpRequestException("Simmy chaos fault"))
            })
            .Build();
    }
}