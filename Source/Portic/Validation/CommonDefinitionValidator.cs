using Portic.Endpoints;
using Portic.Exceptions;
using Portic.Messages;

namespace Portic.Validation
{
    internal static class CommonDefinitionValidator
    {
        public static void ValidateSingleMessageConsumerEndpoint(IEndpointDefinition endpointDefinition)
        {
            // Ensure that only one consumer is registered per message type for this endpoint, 
            // as we shouldn't have multiple consumers consuming the same message type from the same queue in RabbitMQ (the user should use multiple queues)

            HashSet<Type> seenMessageTypes = [];

            foreach (var consumer in endpointDefinition.ConsumerDefinitions)
            {
                if (!seenMessageTypes.Add(consumer.Message.MessageType))
                {
                    throw new MultipleMessageConsumerException(endpointDefinition.Name, consumer.Message.MessageType);
                }
            }
        }

        public static void ValidateMessageDefinitions(IEnumerable<IMessageDefinition> messageDefinitions)
        {
            HashSet<string> seenMessageNames = [];

            foreach (var messageDefinition in messageDefinitions)
            {
                if (!seenMessageNames.Add(messageDefinition.Name))
                {
                    throw new DuplicateMessageNameException(messageDefinition.Name);
                }
            }
        }
    }
}
