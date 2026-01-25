using Portic.Abstractions;
using Portic.Consumer;

namespace Portic.Configuration
{
    internal sealed class MessageConsumerConfigurator(IPorticConfigurator configurator, Type consumerType, Type messageType) : IMessageConsumerConfigurator
    {
        private readonly IPorticConfigurator _configurator = configurator;

        public Type ConsumerType { get; } = consumerType;
        public Type MessageType { get; } = messageType;

        public string EndpointName { get; private set; } = messageType.FullName ?? messageType.Name;

        public IMessageConsumerConfigurator WithEndpointName(string endpointName)
        {
            EndpointName = endpointName;

            _configurator.ConfigureEndpoint(endpointName);

            return this;
        }

        public IConsumerConfiguration Build(IMessageConfiguration message)
        {
            return new ConsumerConfiguration(
                message,
                ConsumerType,
                EndpointName
            );
        }
    }
}