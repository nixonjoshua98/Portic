using Portic.Abstractions;
using Portic.Consumer;
using Portic.Endpoint;
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

        public IEndpointConfiguration EndpointConfiguration { get; }
        public IConsumerConfiguration ConsumerConfiguration { get; }
        public IMessageConfiguration MessageConfiguration { get; }

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
