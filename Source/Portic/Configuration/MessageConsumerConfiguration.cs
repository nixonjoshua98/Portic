using Portic.Abstractions;
using Portic.Consumer;

namespace Portic.Configuration
{
    internal sealed class MessageConsumerConfiguration(
        IMessageConfiguration message,
        Type consumer,
        string endpointName
    ) : IMessageConsumerConfiguration
    {
        public IMessageConfiguration Message { get; } = message;
        public Type ConsumerType { get; } = consumer;
        public string EndpointName { get; } = endpointName;
    }
}