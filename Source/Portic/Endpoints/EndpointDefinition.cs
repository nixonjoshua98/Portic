using Portic.Consumers;
using Portic.Exceptions;
using Portic.Models;

namespace Portic.Endpoints
{
    internal sealed class EndpointDefinition(
        string name,
        IEnumerable<IConsumerDefinition> consumers,
        IReadOnlyCustomPropertyBag properties,
        byte maxRedeliveryAttempts
    ) : IEndpointDefinition
    {
        private readonly IReadOnlyCustomPropertyBag Properties = properties;

        public string Name { get; } = name;
        public byte MaxRedeliveryAttempts { get; } = maxRedeliveryAttempts;
        public IReadOnlyDictionary<string, IConsumerDefinition> Consumers { get; } = consumers.ToDictionary(x => x.Message.Name);

        public T GetPropertyOrDefault<T>(string key, T defaultValue) => Properties.GetOrDefault(key, defaultValue);

        public IConsumerDefinition GetConsumerConfiguration(string? messageName)
        {
            if (string.IsNullOrEmpty(messageName) || !Consumers.TryGetValue(messageName, out var consumer))
            {
                throw MessageTypeNotFoundException.FromName(messageName);
            }

            return consumer;
        }
    }
}