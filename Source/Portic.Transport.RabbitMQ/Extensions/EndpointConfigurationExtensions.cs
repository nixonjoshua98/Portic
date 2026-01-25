using Portic.Endpoint;
using Portic.Transport.RabbitMQ.Channel;

namespace Portic.Transport.RabbitMQ.Extensions
{
    public static class EndpointConfigurationExtensions
    {
        const string PrefetchCountKey = "rmq-prefetch-count";
        const string ChannelCountKey = "rmq-channel-count";
        const string AutoDeleteKey = "rmq-auto-delete";
        const string DurableKey = "rmq-durable";

        public static IEndpointConfigurator WithPrefetchCount(this IEndpointConfigurator configurator, ushort value)
        {
            return configurator.SetProperty(PrefetchCountKey, value);
        }

        public static IEndpointConfigurator WithAutoDelete(this IEndpointConfigurator configurator, bool value = true) =>
            configurator.SetProperty(AutoDeleteKey, value);
        internal static bool GetAutoDelete(this IEndpointConfiguration configurator) =>
            configurator.GetPropertyOrDefault(AutoDeleteKey, false);

        public static IEndpointConfigurator WithDurable(this IEndpointConfigurator configurator, bool value = true) =>
            configurator.SetProperty(DurableKey, value);
        internal static bool GetDurable(this IEndpointConfiguration configurator) =>
            configurator.GetPropertyOrDefault(DurableKey, true);

        public static IEndpointConfigurator WithChannelCount(this IEndpointConfigurator configurator, byte value)
        {
            return configurator.SetProperty(ChannelCountKey, value);
        }

        internal static ushort GetPrefetchCount(this IEndpointConfiguration configurator)
        {
            return configurator.GetPropertyOrDefault<ushort>(PrefetchCountKey, 1);
        }

        internal static byte GetChannelCount(this IEndpointConfiguration configurator)
        {
            return configurator.GetPropertyOrDefault<byte>(ChannelCountKey, 1);
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
