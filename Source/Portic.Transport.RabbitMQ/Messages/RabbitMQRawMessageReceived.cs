using Portic.Consumers;
using Portic.Endpoints;
using Portic.Messages;
using Portic.Transport.RabbitMQ.Consumers;
using Portic.Transport.RabbitMQ.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Portic.Transport.RabbitMQ.Messages
{
    internal sealed class RabbitMQRawMessageReceived
    {
        public IChannel Channel { get; }
        public ulong DeliveryTag { get; }

        public IEndpointDefinition EndpointDefinition { get; }
        public IConsumerDefinition ConsumerDefinition { get; }
        public IMessageDefinition MessageDefinition { get; }

        public ReadOnlyMemory<byte> RawBody { get; }
        public IReadOnlyBasicProperties BasicProperties { get; }

        public string? MessageId => BasicProperties.PorticMessageId;
        public byte DeliveryCount => BasicProperties.DeliveryCount;

        public RabbitMQRawMessageReceived(RabbitMQEndpointConsumerState state, BasicDeliverEventArgs deliverArgs)
        {
            Channel = state.GetChannelOrThrow();

            ConsumerDefinition = state.Endpoint.GetConsumerDefinition(deliverArgs.BasicProperties.MessageName);

            RawBody = deliverArgs.Body;
            EndpointDefinition = state.Endpoint;
            DeliveryTag = deliverArgs.DeliveryTag;
            BasicProperties = deliverArgs.BasicProperties;
            MessageDefinition = ConsumerDefinition.Message;
        }
    }
}