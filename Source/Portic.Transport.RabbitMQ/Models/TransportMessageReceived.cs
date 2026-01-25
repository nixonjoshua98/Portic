using Portic.Consumer;
using Portic.Endpoint;
using Portic.Transport.RabbitMQ.Consumer;
using Portic.Transport.RabbitMQ.Extensions;
using RabbitMQ.Client.Events;
using System.Diagnostics.CodeAnalysis;

namespace Portic.Transport.RabbitMQ.Models
{
    internal sealed class TransportMessageReceived(
        RabbitMQEndpointState state, 
        BasicDeliverEventArgs deliverArgs
    )
    {
        public readonly RabbitMQEndpointState State = state;

        public readonly IEndpointConfiguration EndpointConfiguration = state.Endpoint;

        private readonly BasicDeliverEventArgs DeliverArgs = deliverArgs;

        public readonly string? MessageName = deliverArgs.GetMessageName();

        public ReadOnlySpan<byte> Body => DeliverArgs.Body.Span;

        public bool TryGetConsumerConfiguration([NotNullWhen(true)] out IConsumerConfiguration? consumer)
        {
            return EndpointConfiguration.TryGetConsumerForMessage(MessageName!, out consumer);
        }
    }
}
