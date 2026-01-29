using Polly;
using Polly.Retry;
using Portic.Middleware.Polly.Extensions;

namespace Portic.Middleware.Polly.Configuration
{
    internal sealed class PollyMiddlewareConfigurator : IPollyMiddlewareConfigurator
    {
        public ResiliencePipelineBuilder Builder { get; } = new ResiliencePipelineBuilder();

        public bool UseScopePerExecution { get; private set; } = false;

        public IPollyMiddlewareConfigurator WithScopePerExecution(bool value = true)
        {
            UseScopePerExecution = value;
            return this;
        }

        public IPollyMiddlewareConfigurator WithRetryCount(byte retryCount, TimeSpan? delay = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount, nameof(retryCount));

            Builder.AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = retryCount,
                Delay = delay ?? TimeSpan.FromMilliseconds(500),
                OnRetry = args => OnPollyPolicyRetryAsync(retryCount, args)
            });

            return this;
        }

        private static ValueTask OnPollyPolicyRetryAsync(int retryCount, OnRetryArguments<object> args)
        {
            if (args.Context.TryGetLoggingProperties(out var context, out var logger))
            {
                var delay = args.RetryDelay.TotalMilliseconds;

                logger.LogRetryAttempt(context.MessageId, args.AttemptNumber + 1, retryCount, delay);
            }

            return ValueTask.CompletedTask;
        }
    }
}
