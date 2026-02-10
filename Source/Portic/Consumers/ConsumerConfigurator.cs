using Portic.Configuration;
using Portic.Messages;

namespace Portic.Consumers
{
    internal sealed class ConsumerConfigurator(IPorticConfigurator configurator, Type consumerType, Type messageType) : IConsumerConfigurator
    {
        private readonly IPorticConfigurator _configurator = configurator;

        public Type ConsumerType { get; } = consumerType;
        public Type MessageType { get; } = messageType;

        public string EndpointName { get; private set; } = consumerType.FullName ?? consumerType.Name;

        public IConsumerConfigurator WithEndpointName(string endpointName)
        {
            ArgumentException.ThrowIfNullOrEmpty(endpointName, nameof(endpointName));

            EndpointName = endpointName;

            _configurator.ConfigureEndpoint(endpointName);

            return this;
        }

        public IConsumerDefinition Build(IMessageDefinition message)
        {
            return new ConsumerDefinition(
                message,
                ConsumerType,
                EndpointName
            );
        }
    }
}