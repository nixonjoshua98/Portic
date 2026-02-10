using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portic.Configuration;
using Portic.Transport.InMemory.Consumers;
using Portic.Transport.InMemory.Topology;

namespace Portic.Transport.InMemory.Extensions
{
    public static class SetupExtensions
    {
        public static IPorticConfigurator UsingInMemory(this IPorticConfigurator builder, Action<IInMemoryTransportConfigurator>? callback = null)
        {
            var busBuilder = new InMemoryTransportDefinition();

            callback?.Invoke(busBuilder);

            var transportDefinition = busBuilder.ToDefinition();

            builder.SetTransportDefinition(transportDefinition);

            builder.Services.TryAddSingleton(transportDefinition);

            AddCoreServices(builder.Services);

            return builder;
        }

        private static void AddCoreServices(IServiceCollection services)
        {
            services.TryAddSingleton<IInMemoryTransport, InMemoryTransport>();

            services.TryAddSingleton<IMessageTransport>(
                provider => provider.GetRequiredService<IInMemoryTransport>()
            );

            services.TryAddSingleton<InMemoryConsumerExecutor>();

            services.AddHostedService<InMemoryBackgroundDequeuer>();
        }
    }
}