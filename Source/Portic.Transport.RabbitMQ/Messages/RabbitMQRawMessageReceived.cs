using Portic.Consumers;
using Portic.Endpoints;
using Portic.Exceptions;
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

            ConsumerDefinition = GetConsumerDefinition(state.Endpoint, deliverArgs.BasicProperties.MessageName);

            RawBody = deliverArgs.Body;
            EndpointDefinition = state.Endpoint;
            DeliveryTag = deliverArgs.DeliveryTag;
            BasicProperties = deliverArgs.BasicProperties;
            MessageDefinition = ConsumerDefinition.Message;
        }

        private static IConsumerDefinition GetConsumerDefinition(IEndpointDefinition endpointDefinition, string? messageName)
        {
            var consumerDefinition = endpointDefinition.ConsumerDefinitions
                .SingleOrDefault(c => c.Message.Name == messageName);

            if (string.IsNullOrEmpty(messageName) || consumerDefinition is null)
            {
                throw new MessageConsumerNotConfiguredException(messageName, endpointDefinition.Name);
            }

            return consumerDefinition;
        }
    }
}