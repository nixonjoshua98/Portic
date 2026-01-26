using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portic.Abstractions;
using Portic.Transport.RabbitMQ.Configuration;
using Portic.Transport.RabbitMQ.Consumer;
using Portic.Transport.RabbitMQ.Topology;

namespace Portic.Transport.RabbitMQ.Extensions
{
    public static class ConsumerBuilderExtensions
    {
        public static IPorticConfigurator UsingRabbitMQ(this IPorticConfigurator builder, Action<IRabbitMQTransportConfigurator>? callback = null)
        {
            var busBuilder = new RabbitMQTransportConfiguration(builder);

            callback?.Invoke(busBuilder);

            builder.Services.TryAddSingleton(busBuilder.Build());

            AddCoreServices(builder.Services);

            return builder;
        }

        private static void AddCoreServices(IServiceCollection services)
        {
            services.AddHostedService<RabbitMQTopologyHostedService>();

            services.TryAddSingleton<IMessageTransport, RabbitMQTransport>();

            services.TryAddSingleton<IRabbitMQConsumerExecutor, RabbitMQConsumerExecutor>();

            services.TryAddSingleton<IRabbitMQTopologyFactory, RabbitMQTopologyFactory>();

            services.TryAddSingleton<IRabbitMQConnectionContext, RabbitMQConnectionContext>();
        }
    }
}