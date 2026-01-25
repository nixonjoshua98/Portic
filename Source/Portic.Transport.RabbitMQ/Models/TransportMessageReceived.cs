using Portic.Transport.RabbitMQ.Consumer;
using Portic.Transport.RabbitMQ.Extensions;
using RabbitMQ.Client.Events;

namespace Portic.Transport.RabbitMQ.Models
{
    internal sealed class TransportMessageReceived(
        RabbitMQEndpointState endpoint, 
        BasicDeliverEventArgs deliverArgs
    )
    {
        public readonly RabbitMQEndpointState Endpoint = endpoint;

        private readonly BasicDeliverEventArgs DeliverArgs = deliverArgs;

        public readonly string? MessageName = deliverArgs.GetMessageName();

        public ReadOnlySpan<byte> Body => DeliverArgs.Body.Span;
    }
}
