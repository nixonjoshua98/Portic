using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using Portic.Abstractions;
using Portic.Middleware.Polly.Configuration;
using Portic.Middleware.Polly.Middleware;

namespace Portic.Middleware.Polly.Extensions
{
    public static class PollyMiddlewareExtensions
    {
        private const string PollyMiddlewareAdded = "polly-middleware";

        /// <summary>
        /// Configures the retry policy to use the specified maximum number of retry attempts for transient failures.
        /// </summary>
        /// <remarks>This method adds a retry policy to the configuration, allowing operations to be
        /// retried up to the specified number of times in case of transient errors. If a retry policy is already
        /// configured, it will be replaced.</remarks>
        /// <param name="configurator">The configurator instance to apply the retry policy to.</param>
        /// <param name="retryCount">The maximum number of retry attempts to use. Must be greater than zero.</param>
        /// <param name="delay">An optional delay between retry attempts. If not provided, a default delay of 500 milliseconds will be used.</param>
        /// <returns>The same configurator instance, enabling method chaining.</returns>
        public static IPorticConfigurator UseRetryCount(this IPorticConfigurator configurator, byte retryCount, TimeSpan? delay = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount, nameof(retryCount));

            return UsePolly(configurator, config =>
            {
                config.Builder.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = retryCount,
                    Delay = delay ?? TimeSpan.FromMilliseconds(500),
                    OnRetry = args => OnPollyPolicyRetryAsync(retryCount, args)
                });
            });
        }

        private static IPorticConfigurator UsePolly(IPorticConfigurator configurator, Action<IPollyMiddlewareConfigurator> callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));

            var pollyConfigurator = new PollyMiddlewareConfigurator();

            callback(pollyConfigurator);

            var pipeline = pollyConfigurator.Builder.Build();

            return UsePolly(configurator, pipeline);
        }

        private static void UsePollyMiddleware(IPorticConfigurator configurator)
        {
            if (!configurator.HasProperty(PollyMiddlewareAdded))
            {
                configurator.Use<PollyMiddleware>();
                configurator.SetProperty(PollyMiddlewareAdded, string.Empty);
            }
        }

        private static IPorticConfigurator UsePolly(IPorticConfigurator configurator, ResiliencePipeline pipeline)
        {
            ArgumentNullException.ThrowIfNull(pipeline, nameof(pipeline));

            configurator.Services.Configure<PollyMiddlewareOptions>(options =>
            {
                options.Pipeline = pipeline;
            });

            UsePollyMiddleware(configurator);

            return configurator;
        }

        private static ValueTask OnPollyPolicyRetryAsync(int retryCount, OnRetryArguments<object> args)
        {
            if (args.Context.TryGetLoggingProperties(out var context, out var logger))
            {
                var delay = args.RetryDelay.TotalMilliseconds;

                logger.LogRetryAttempt(context.MessageId, context.MessageName, args.AttemptNumber + 1, retryCount, delay);
            }

            return ValueTask.CompletedTask;
        }
    }
}
