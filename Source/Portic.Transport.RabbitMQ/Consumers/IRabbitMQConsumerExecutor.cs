using Portic.Transport.RabbitMQ.Messages;

namespace Portic.Transport.RabbitMQ.Consumers
{
    internal interface IRabbitMQConsumerExecutor
    {
        Task ExecuteAsync(RabbitMQRawMessageReceived message, CancellationToken cancellationToken);
    }
}
