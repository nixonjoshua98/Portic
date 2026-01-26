using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portic.Abstractions;
using Portic.Configuration;
using Portic.Consumer;
using Portic.Serializer;

namespace Portic.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPortic(this IServiceCollection services, Action<IPorticConfigurator> callback)
        {
            var builder = new PorticConfigurator(services);

            callback(builder);

            services.TryAddSingleton(builder.Build());

            AddCoreServices(services);

            return services;
        }

        private static void AddCoreServices(IServiceCollection services)
        {
            services.TryAddSingleton<IConsumerExecutor, ConsumerExecutor>();

            services.TryAddSingleton<IPorticSerializer, SystemTextJsonSerializer>();
        }
    }
}
