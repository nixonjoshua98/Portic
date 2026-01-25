using Portic.Abstractions;

namespace Portic.Consumer
{
    public interface IMessageConsumerConfiguration
    {
        IMessageConfiguration Message { get; }
        Type ConsumerType { get; }

        string GetQueueName();
    }
}