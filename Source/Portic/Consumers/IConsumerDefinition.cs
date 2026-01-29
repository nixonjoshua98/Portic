using Portic.Messages;

namespace Portic.Consumers
{
    public interface IConsumerDefinition
    {
        IMessageDefinition Message { get; }
        internal Type ConsumerType { get; }
        internal string EndpointName { get; }
    }
}