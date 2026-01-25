using Portic.Consumer;
using Portic.Endpoint;
using Portic.Transport.RabbitMQ.Extensions;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Extensions
{
    internal static class ChannelExtensions
    {
        public static async Task<QueueDeclareOk> QueueDeclareAsync(this IChannel channel, IEndpointConfiguration endpoint, CancellationToken cancellationToken)
        {
            return await channel.QueueDeclareAsync(
                endpoint.Name,
                exclusive: true,
                autoDelete: true,
                cancellationToken: cancellationToken
            );
        }
    }
}
