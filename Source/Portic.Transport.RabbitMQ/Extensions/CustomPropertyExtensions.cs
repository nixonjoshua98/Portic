using Portic.Endpoints;
using Portic.Messages;
using Portic.Transport.RabbitMQ.Channel;

namespace Portic.Transport.RabbitMQ.Extensions
{
    public static class CustomPropertyExtensions
    {
        private const string PrefetchCountKey = "rmq-prefetchcount";
        private const string ChannelCountKey = "rmq-channelcount";
        private const string AutoDeleteKey = "rmq-autodelete";
        private const string ExclusiveKey = "rmq-exclusive";
        private const string MandatoryKey = "rmq-mandatory";
        private const string DurableKey = "rmq-durable";

        extension(IMessageDefinition configurator)
        {
            internal bool Mandatory =>
                configurator.GetPropertyOrDefault(MandatoryKey, false);
        }

        extension(IMessageConfigurator configurator)
        {
            public IMessageConfigurator WithMandatory(bool value = true) =>
                configurator.SetProperty(MandatoryKey, value);
        }

        extension(IEndpointConfigurator configurator)
        {
            public IEndpointConfigurator WithPrefetchCount(ushort value) =>
                configurator.SetProperty(PrefetchCountKey, value);

            public IEndpointConfigurator WithAutoDelete(bool value = true) =>
                configurator.SetProperty(AutoDeleteKey, value);

            public IEndpointConfigurator WithDurable(bool value = true) =>
                configurator.SetProperty(DurableKey, value);

            public IEndpointConfigurator WithChannelCount(byte value) =>
                configurator.SetProperty(ChannelCountKey, value);

            public IEndpointConfigurator WithExclusive(bool value = true) =>
                configurator.SetProperty(ExclusiveKey, value);
        }

        extension(IEndpointDefinition configurator)
        {
            internal ushort PrefetchCount =>
                configurator.GetPropertyOrDefault<ushort>(PrefetchCountKey, 1);

            internal bool AutoDelete =>
                configurator.GetPropertyOrDefault(AutoDeleteKey, false);

            internal bool Durable =>
                configurator.GetPropertyOrDefault(DurableKey, true);

            internal byte ChannelCount =>
                configurator.GetPropertyOrDefault<byte>(ChannelCountKey, 1);

            internal bool Exclusive =>
                configurator.GetPropertyOrDefault(ExclusiveKey, false);
        }

        extension(IEndpointDefinition endpoint)
        {
            internal RabbitMQChannelOptions CreateChannelOptions()
            {
                return new RabbitMQChannelOptions(
                    0,
                    endpoint.PrefetchCount
                );
            }
        }
    }
}
