using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portic.Configuration;
using Portic.Consumers;
using Portic.Endpoints;
using Portic.Serializer;

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

            AddCoreServices(services);

            return services;
        }

        private static void AddCoreServices(IServiceCollection services)
        {
            services.AddHostedService<ReceiveEndpointBackgroundService>();

            services.TryAddSingleton<IConsumerExecutor, ConsumerExecutor>();

            services.TryAddSingleton<IPorticSerializer, SystemTextJsonSerializer>();
        }
    }
}
