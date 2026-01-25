using Portic.Endpoint;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Extensions
{
    internal static class ChannelExtensions
    {
        public static async Task<QueueDeclareOk> QueueDeclareAsync(this IChannel channel, IEndpointConfiguration endpoint, CancellationToken cancellationToken)
        {
            return await channel.QueueDeclareAsync(
                queue: endpoint.Name,
                durable: endpoint.GetDurable(),
                exclusive: false,
                autoDelete: endpoint.GetAutoDelete(),
                cancellationToken: cancellationToken
            );
        }
    }
}
