using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTopologyService(RabbitMQConnectionContext _connectionContext)
    {
        public async Task BindFaultedQueueAsync(string exchange, string queue, CancellationToken cancellationToken)
        {
            using var rented = await _connectionContext.RentChannelAsync(cancellationToken);

            await rented.Channel.ExchangeDeclareAsync(
                exchange: exchange,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            await rented.Channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            await rented.Channel.QueueBindAsync(
                queue: queue, 
                exchange: exchange, 
                routingKey: string.Empty, 
                cancellationToken: cancellationToken
            );
        }
    }
}
