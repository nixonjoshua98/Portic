using Portic.Transport.RabbitMQ.Models;

namespace Portic.Transport.RabbitMQ.Consumer
{
    internal interface IRabbitMQMessageConsumer
    {
        Task ConsumeAsync(TransportMessageReceived message, CancellationToken cancellationToken);
    }
}
