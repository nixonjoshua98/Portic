using Portic.Endpoint;
using Portic.Transport.RabbitMQ.Channel;

namespace Portic.Transport.RabbitMQ.Extensions
{
    public static class EndpointConfigurationExtensions
    {
        const string PrefetchCountKey = "rmq-prefetch-count";

        public static IEndpointConfigurator WithPrefetchCount(this IEndpointConfigurator configurator, ushort preFetchCount)
        {
            configurator.SetProperty(PrefetchCountKey, preFetchCount);

            return configurator;
        }

        internal static ushort GetPrefetchCount(this IEndpointConfiguration configurator)
        {
            return configurator.GetPropertyOrDefault<ushort>(PrefetchCountKey, 1);
        }

        internal static RabbitMQChannelOptions CreateChannelOptions(this IEndpointConfiguration endpoint)
        {
            return new RabbitMQChannelOptions(
                0, 
                endpoint.GetPrefetchCount()
            );
        }
    }
}
