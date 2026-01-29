using Portic.Consumers;
using Portic.Endpoints;
using Portic.Messages;
using Portic.Transport.RabbitMQ.Consumer;
using Portic.Transport.RabbitMQ.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Portic.Transport.RabbitMQ.Models
{
    internal sealed class RawTransportMessageReceived
    {
        public IChannel Channel { get; }
        public byte DeliveryCount { get; }
        public ulong DeliveryTag { get; }

        public IEndpointDefinition EndpointConfiguration { get; }
        public IConsumerDefinition ConsumerConfiguration { get; }
        public IMessageDefinition MessageConfiguration { get; }

        public ReadOnlyMemory<byte> RawBody { get; }

        public RawTransportMessageReceived(RabbitMQEndpointConsumerState state, BasicDeliverEventArgs deliverArgs)
        {
            Channel = state.GetChannelOrThrow();

            ConsumerConfiguration = state.Endpoint.GetConsumerConfiguration(deliverArgs.BasicProperties.MessageName);

            RawBody = deliverArgs.Body;
            EndpointConfiguration = state.Endpoint;
            DeliveryCount = deliverArgs.BasicProperties.DeliveryCount;
            DeliveryTag = deliverArgs.DeliveryTag;
            MessageConfiguration = ConsumerConfiguration.Message;
        }

        public TransportMessageReceived<TMessage> ToReceivedMessage<TMessage>(string messageId, TMessage message)
        {
            return new TransportMessageReceived<TMessage>(
                messageId,
                message,
                DeliveryCount,
                ConsumerConfiguration,
                EndpointConfiguration
            );
        }
    }
}
