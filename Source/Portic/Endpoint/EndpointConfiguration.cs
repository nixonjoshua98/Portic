using Portic.Consumer;
using Portic.Exceptions;
using Portic.Models;

namespace Portic.Endpoint
{
    internal sealed class EndpointConfiguration(
        string name,
        IEnumerable<IConsumerConfiguration> consumers,
        IReadOnlyCustomPropertyBag properties,
        byte maxRedeliveryAttempts
    ) : IEndpointConfiguration
    {
        private readonly IReadOnlyCustomPropertyBag Properties = properties;

        public string Name { get; } = name;
        public byte MaxRedeliveryAttempts { get; } = maxRedeliveryAttempts;
        public IReadOnlyDictionary<string, IConsumerConfiguration> Consumers { get; } = consumers.ToDictionary(x => x.Message.Name);

        public T GetPropertyOrDefault<T>(string key, T defaultValue) => Properties.GetOrDefault(key, defaultValue);

        public IConsumerConfiguration GetConsumerConfiguration(string? messageName)
        {
            if (string.IsNullOrEmpty(messageName) || !Consumers.TryGetValue(messageName, out var consumer))
            {
                throw MessageTypeNotFoundException.FromName(messageName);
            }

            return consumer;
        }
    }
}