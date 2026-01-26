using Portic.Transport.RabbitMQ.Models;

namespace Portic.Transport.RabbitMQ.Consumer
{
    internal interface IRabbitMQConsumerExecutor
    {
        Task ExecuteAsync(TransportMessageReceived message, CancellationToken cancellationToken);
    }
}
