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
        const string PollyMiddlewareAdded = "polly-middleware-added";

        public static IPorticConfigurator UsePolly(this IPorticConfigurator configurator, ResiliencePipeline pipeline)
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
                    MaxRetryAttempts = retryCount
                });
            });
        }
    }
}
