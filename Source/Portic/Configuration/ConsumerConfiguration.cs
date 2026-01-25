using Portic.Abstractions;
using Portic.Consumer;

namespace Portic.Configuration
{
    internal sealed class ConsumerConfiguration(
        IMessageConfiguration message,
        Type consumer,
        string endpointName
    ) : IConsumerConfiguration
    {
        public IMessageConfiguration Message { get; } = message;
        public Type ConsumerType { get; } = consumer;
        public string EndpointName { get; } = endpointName;
    }
}