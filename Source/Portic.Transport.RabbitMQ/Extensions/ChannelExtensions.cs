using Portic.Consumer;
using Portic.Transport.RabbitMQ.Extensions;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Extensions
{
    internal static class ChannelExtensions
    {
        public static async Task<QueueDeclareOk> QueueDeclareAsync(this IChannel channel, IMessageConsumerConfiguration configuration, CancellationToken cancellationToken)
        {
            return await channel.QueueDeclareAsync(
                configuration.GetQueueName(),
                exclusive: true,
                autoDelete: true,
                cancellationToken: cancellationToken
            );
        }
    }
}
