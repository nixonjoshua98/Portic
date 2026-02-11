using Portic.Consumers;
using Portic.Endpoints;
using Portic.Exceptions;
using Portic.Messages;
using Portic.Transport.RabbitMQ.Channels;
using Portic.Transport.RabbitMQ.Consumers;
using Portic.Transport.RabbitMQ.Extensions;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Messages
{
    internal sealed class RabbitMQRawMessageReceived
    {
        public RabbitMQChannel Channel { get; }
        public ulong DeliveryTag { get; }

        public IEndpointDefinition EndpointDefinition { get; }
        public IConsumerDefinition ConsumerDefinition { get; }
        public IMessageDefinition MessageDefinition { get; }

        public ReadOnlyMemory<byte> RawBody { get; }
        public IReadOnlyBasicProperties BasicProperties { get; }

        public string? MessageId => BasicProperties.PorticMessageId;
        public byte DeliveryCount => BasicProperties.DeliveryCount;

        public RabbitMQRawMessageReceived(
            RabbitMQBasicConsumer state,
            ReadOnlyMemory<byte> body,
            ulong deliveryTag,
            IReadOnlyBasicProperties basicProperties)
        {
            ConsumerDefinition = GetConsumerDefinition(state.Endpoint, basicProperties.MessageName);

            RawBody = body;
            Channel = state.Channel;
            DeliveryTag = deliveryTag;
            BasicProperties = basicProperties;
            EndpointDefinition = state.Endpoint;
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