using Portic.Consumer;
using Portic.Endpoint;
using Portic.Transport.RabbitMQ.Consumer;
using Portic.Transport.RabbitMQ.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics.CodeAnalysis;

namespace Portic.Transport.RabbitMQ.Models
{
    internal sealed class TransportMessageReceived(
        RabbitMQEndpointConsumerState state,
        BasicDeliverEventArgs deliverArgs
    )
    {
        public readonly IChannel Channel = state.Channel;

        public readonly IEndpointConfiguration EndpointConfiguration = state.Endpoint;

        public readonly string? MessageName = deliverArgs.GetMessageName();

        public readonly ulong DeliveryTag = deliverArgs.DeliveryTag;

        public ReadOnlySpan<byte> Body => deliverArgs.Body.Span;

        public bool TryGetConsumerConfiguration([NotNullWhen(true)] out IConsumerConfiguration? consumer)
        {
            return EndpointConfiguration.TryGetConsumerForMessage(MessageName!, out consumer);
        }
    }
}
