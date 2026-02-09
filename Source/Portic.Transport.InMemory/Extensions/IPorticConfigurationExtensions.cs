using Portic.Configuration;
using Portic.Messages;
using Portic.Transport.InMemory.Consumers;

namespace Portic.Transport.InMemory.Extensions
{
    internal static class IPorticConfigurationExtensions
    {
        public static IEnumerable<EndpointConsumerPair> GetConsumers(this IPorticConfiguration configuration, IMessageDefinition messageDefinition)
        {
            foreach (var endpointDefinition in configuration.Endpoints)
            {
                foreach (var consumerDefinition in endpointDefinition.ConsumerDefinitions)
                {
                    if (consumerDefinition.Message == messageDefinition)
                    {
                        yield return new(endpointDefinition, consumerDefinition);
                    }
                }
            }
        }
    }
}
