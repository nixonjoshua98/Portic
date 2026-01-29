using Portic.Endpoints;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Extensions
{
    internal static class ChannelExtensions
    {
        public static async Task<QueueDeclareOk> QueueDeclareAsync(this IChannel channel, IEndpointDefinition endpoint, CancellationToken cancellationToken)
        {
            return await channel.QueueDeclareAsync(
                queue: endpoint.Name,
                durable: endpoint.Durable,
                exclusive: endpoint.Exclusive,
                autoDelete: endpoint.AutoDelete,
                cancellationToken: cancellationToken
            );
        }
    }
}
