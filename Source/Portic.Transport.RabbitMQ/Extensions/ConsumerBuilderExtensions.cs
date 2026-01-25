using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portic.Abstractions;
using Portic.Transport.RabbitMQ.Abstractions;
using Portic.Transport.RabbitMQ.Configuration;
using Portic.Transport.RabbitMQ.Consumer;
using Portic.Transport.RabbitMQ.Topology;

namespace Portic.Transport.RabbitMQ.Extensions
{
    public static class ConsumerBuilderExtensions
    {
        public static IPorticConfigurator UsingRabbitMQ(this IPorticConfigurator builder, Action<IRabbitMQTransportConfigurator> callback)
        {
            var busBuilder = new Configuration.RabbitMQTransport(builder);

            callback(busBuilder);

            builder.Services.TryAddSingleton(busBuilder.Build());

            AddCoreServices(builder.Services);

            return builder;
        }

        static void AddCoreServices(IServiceCollection services)
        {
            services.AddHostedService<RabbitMQTopologyHostedService>();

            services.TryAddSingleton<IMessageTransport, Topology.RabbitMQTransport>();

            services.TryAddSingleton<IRabbitMQMessageConsumer, RabbitMQMessageConsumer>();

            services.TryAddSingleton<IRabbitMQTopologyFactory, RabbitMQTopologyFactory>();

            services.TryAddSingleton<IRabbitMQConnectionContext, RabbitMQConnectionContext>();
        }
    }
}