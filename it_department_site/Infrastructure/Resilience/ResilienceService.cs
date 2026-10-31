using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace it_department_site.Infrastructure.Resilience
{
    public class ResilienceService : IResilienceService
    {
        private readonly ResiliencePipeline _pipeline;

        public ResilienceService(ILogger<ResilienceService>? logger = null)
        {
            _pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(100),
                    BackoffType = DelayBackoffType.Exponential,
                    OnRetry = args =>
                    {
                        logger?.LogWarning("Resilience Retry triggered. Attempt #{AttemptNumber}, Outcome: {Outcome}",
                            args.AttemptNumber, args.Outcome.Exception?.Message);
                        return ValueTask.CompletedTask;
                    }
                })
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = 3,
                    BreakDuration = TimeSpan.FromSeconds(5),
                    OnOpened = args =>
                    {
                        logger?.LogError("Resilience Circuit Breaker OPENED. BreakDuration: {BreakDuration}", args.BreakDuration);
                        return ValueTask.CompletedTask;
                    },
                    OnClosed = args =>
                    {
                        logger?.LogInformation("Resilience Circuit Breaker CLOSED (Normal operation resumed).");
                        return ValueTask.CompletedTask;
                    }
                })
                .AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(2),
                    OnTimeout = args =>
                    {
                        logger?.LogWarning("Resilience Timeout triggered after {Timeout}", args.Timeout);
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();
        }

        public async Task<T> ExecuteWithResilienceAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
        {
            return await _pipeline.ExecuteAsync(async token => await action(token), cancellationToken);
        }
    }
}
