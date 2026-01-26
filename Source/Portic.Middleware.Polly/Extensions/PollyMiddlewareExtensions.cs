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
        private const string PollyMiddlewareAdded = "polly-middleware-added";

        private static IPorticConfigurator UsePolly(IPorticConfigurator configurator, ResiliencePipeline pipeline)
        {
            ArgumentNullException.ThrowIfNull(pipeline, nameof(pipeline));

            configurator.Services.Configure<PollyMiddlewareOptions>(options =>
            {
                options.Pipeline = pipeline;
            });

            // We only ever want a single polly middleware registered
            if (!configurator.HasProperty(PollyMiddlewareAdded))
            {
                configurator.Use<PollyMiddleware>();

                configurator.SetProperty(PollyMiddlewareAdded, string.Empty);
            }

            return configurator;
        }

        public static IPorticConfigurator UsePolly(this IPorticConfigurator configurator, Action<IPollyMiddlewareConfigurator> callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));

            var pollyConfigurator = new PollyMiddlewareConfigurator();

            callback(pollyConfigurator);

            var pipeline = pollyConfigurator.Builder.Build();

            return UsePolly(configurator, pipeline);
        }

        public static IPorticConfigurator UsePolly(this IPorticConfigurator configurator, int retryCount)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount, nameof(retryCount));

            return UsePolly(configurator, config =>
            {
                config.Builder.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = retryCount,
                    OnRetry = args => OnPollyPolicyRetryAsync(retryCount, args)
                });
            });
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
