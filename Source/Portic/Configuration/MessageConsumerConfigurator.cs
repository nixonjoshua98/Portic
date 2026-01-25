using Portic.Abstractions;
using Portic.Consumer;

namespace Portic.Configuration
{
    internal sealed class MessageConsumerConfigurator : IMessageConsumerConfigurator
    {
        private readonly IPorticConfigurator _configurator;

        public Type ConsumerType { get; }
        public Type MessageType { get; }

        public string EndpointName { get; private set; }

        public MessageConsumerConfigurator(IPorticConfigurator configurator, Type consumerType, Type messageConfigurator)
        {
            _configurator = configurator;

            ConsumerType = consumerType;
            MessageType = messageConfigurator;

            EndpointName = messageConfigurator.FullName
                ?? throw new InvalidOperationException("Message type must have a full name");
        }

        public IMessageConsumerConfigurator WithEndpointName(string endpointName)
        {
            EndpointName = endpointName;

            _configurator.ConfigureEndpoint(endpointName);

            return this;
        }

        public IMessageConsumerConfiguration Build(IMessageConfiguration message)
        {
            return new MessageConsumerConfiguration(
                message, 
                ConsumerType,
                EndpointName
            );
        }
    }
}