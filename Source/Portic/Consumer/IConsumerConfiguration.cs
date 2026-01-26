using Portic.Abstractions;

namespace Portic.Consumer
{
    public interface IConsumerConfiguration
    {
        IMessageConfiguration Message { get; }
        internal Type ConsumerType { get; }
        internal string EndpointName { get; }
    }
}