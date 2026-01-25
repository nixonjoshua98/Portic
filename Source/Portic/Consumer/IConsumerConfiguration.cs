using Portic.Abstractions;

namespace Portic.Consumer
{
    public interface IConsumerConfiguration
    {
        IMessageConfiguration Message { get; }
        Type ConsumerType { get; }
        string EndpointName { get; }
    }
}