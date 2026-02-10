using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portic.Configuration;
using Portic.Transport.InMemory.Consumers;
using Portic.Transport.InMemory.Topology;

namespace Portic.Transport.InMemory.Extensions
{
    public static class SetupExtensions
    {
        /// <summary>
        /// Configures the transport to use an in-memory message bus for the current builder instance.
        /// </summary>
        /// <remarks>Using in-memory transport is suitable for development, testing, or scenarios where
        /// persistent messaging is not required. Messages are not persisted and are lost when the application
        /// stops.</remarks>
        /// <param name="builder">The builder instance to configure with in-memory transport. Cannot be null.</param>
        /// <param name="callback">An optional callback to further configure the in-memory transport settings. If null, default settings are
        /// used.</param>
        /// <returns>The same builder instance, configured to use in-memory transport.</returns>
        public static IPorticConfigurator UsingInMemory(this IPorticConfigurator builder, Action<IInMemoryTransportConfigurator>? callback = null)
        {
            var busBuilder = new InMemoryTransportDefinition();

            callback?.Invoke(busBuilder);

            var transportDefinition = busBuilder.ToDefinition();

            builder.SetTransportDefinition(transportDefinition);

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