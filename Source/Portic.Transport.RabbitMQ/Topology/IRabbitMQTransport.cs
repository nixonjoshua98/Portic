using Portic.Consumer;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal interface IRabbitMQTransport : IMessageTransport
    {
        Task RePublishAsync<TMessage>(IConsumerContext<TMessage> context, CancellationToken cancellationToken);
    }
}
