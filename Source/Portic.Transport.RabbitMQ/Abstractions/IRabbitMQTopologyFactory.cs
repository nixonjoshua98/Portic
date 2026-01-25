using Portic.Consumer;

namespace Portic.Transport.RabbitMQ.Abstractions
{
    internal interface IRabbitMQTopologyFactory
    {
        Task CreateTopologyAsync(IEnumerable<IMessageConsumerConfiguration> consumers, CancellationToken cancellationToken);
    }
}