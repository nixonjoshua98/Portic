using RabbitMQ.Client.Events;

namespace Portic.Transport.RabbitMQ.Consumer
{
    internal interface IRabbitMQMessageConsumer
    {
        Task ConsumeAsync(BasicDeliverEventArgs args, CancellationToken cancellationToken);
    }
}
