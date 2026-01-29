using Microsoft.Extensions.DependencyInjection;
using Portic.Configuration;
using Portic.Middleware.Polly.Configuration;
using Portic.Middleware.Polly.Middleware;

namespace Portic.Middleware.Polly.Extensions
{
    public static class PollyMiddlewareExtensions
    {
        private const string PollyMiddlewareAdded = "polly-middleware";

        /// <summary>
        /// Configures Polly-based resilience policies for the Portic pipeline using the specified configuration
        /// callback.
        /// </summary>
        /// <param name="configurator">The Portic configurator to which Polly middleware will be added.</param>
        /// <param name="callback">A callback that configures the Polly middleware by providing an <see cref="IPollyMiddlewareConfigurator"/>
        /// instance. Cannot be null.</param>
        /// <returns>The same <see cref="IPorticConfigurator"/> instance for method chaining.</returns>
        public static IPorticConfigurator UsePolly(this IPorticConfigurator configurator, Action<IPollyMiddlewareConfigurator> callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));

            var pollyConfigurator = new PollyMiddlewareConfigurator();

            callback(pollyConfigurator);

            var pipeline = pollyConfigurator.Builder.Build();

            configurator.Services.Configure<PollyMiddlewareOptions>(options =>
            {
                options.Pipeline = pipeline;
                options.UseScopePerExecution = pollyConfigurator.UseScopePerExecution;
            });

            UsePollyMiddleware(configurator);

            return configurator;
        }

        private static void UsePollyMiddleware(IPorticConfigurator configurator)
        {
            if (!configurator.HasProperty(PollyMiddlewareAdded))
            {
                configurator.Use<PollyMiddleware>();
                configurator.SetProperty(PollyMiddlewareAdded, string.Empty);
            }
        }
    }
}
