using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portic.Configuration;
using Portic.Transport.RabbitMQ.Consumers;
using Portic.Transport.RabbitMQ.Topology;

namespace Portic.Transport.RabbitMQ.Extensions
{
    public static class SetupExtensions
    {
        extension(IPorticConfigurator builder)
        {
            /// <summary>
            /// Configures the builder to use RabbitMQ as the transport for messaging and allows optional customization of
            /// the RabbitMQ transport settings.
            /// </summary>
            /// <remarks>Call this method to enable RabbitMQ transport for your application's messaging
            /// infrastructure. The optional callback provides access to advanced RabbitMQ configuration options before the
            /// transport is registered.</remarks>
            /// <param name="builder">The configurator used to register services and configure messaging for the application.</param>
            /// <param name="callback">An optional callback to further configure RabbitMQ transport options. If null, default settings are used.</param>
            /// <returns>The same configurator instance, enabling further configuration chaining.</returns>
            public IPorticConfigurator UsingRabbitMQ(Action<IRabbitMQTransportConfigurator>? callback = null)
            {
                var busBuilder = new RabbitMQTransportDefinition();

                callback?.Invoke(busBuilder);

                var transportDefinition = busBuilder.Build();

                builder.SetTransportDefinition(transportDefinition);

                builder.Services.TryAddSingleton(transportDefinition);

                AddCoreServices(builder.Services);

                return builder;
            }
        }

        private static void AddCoreServices(IServiceCollection services)
        {
            services.TryAddSingleton<IRabbitMQTransport, RabbitMQTransport>();

            services.TryAddSingleton<IMessageTransport>(
                provider => provider.GetRequiredService<IRabbitMQTransport>()
            );

            services.AddHostedService<RabbitMQTopologyHostedService>();

            services.TryAddSingleton<IRabbitMQConsumerExecutor, RabbitMQConsumerExecutor>();

            services.TryAddSingleton<IRabbitMQConnectionContext, RabbitMQConnectionContext>();
        }
    }
}