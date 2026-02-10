using Portic.Consumers;
using Portic.Messages;

namespace Portic.Tests.Common.Mocks
{
    public sealed class MockConsumerDefinition(
        Type consumerType,
        string endpointName,
        IMessageDefinition messageDefinition
    ) : IConsumerDefinition
    {
        public Type ConsumerType { get; } = consumerType;
        public string EndpointName { get; } = endpointName;
        public IMessageDefinition Message { get; } = messageDefinition;
    }
}