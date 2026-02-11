using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portic.Configuration;
using Portic.Consumers;
using Portic.Endpoints;

namespace Portic.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPortic(this IServiceCollection services, Action<IPorticConfigurator> callback)
        {
            var builder = new PorticConfigurator(services);

            callback(builder);

            var definition = builder.Build();

            services.TryAddSingleton(definition);

            services.AddHostedService<ReceiveEndpointBackgroundService>();

            services.TryAddSingleton<IConsumerExecutor, ConsumerExecutor>();

            return services;
        }
    }
}
