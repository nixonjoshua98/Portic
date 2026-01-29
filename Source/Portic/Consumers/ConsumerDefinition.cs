using Portic.Messages;

namespace Portic.Consumers
{
    internal sealed class ConsumerDefinition(
        IMessageDefinition message,
        Type consumer,
        string endpointName
    ) : IConsumerDefinition
    {
        public IMessageDefinition Message { get; } = message;
        public Type ConsumerType { get; } = consumer;
        public string EndpointName { get; } = endpointName;
    }
}