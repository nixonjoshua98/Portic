using Portic.Messages;

namespace Portic.Consumers
{
    public interface IConsumerDefinition
    {
        Type ConsumerType { get; }
        string EndpointName { get; }
        IMessageDefinition Message { get; }
    }
}