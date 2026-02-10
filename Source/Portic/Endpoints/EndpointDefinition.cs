using Portic.Consumers;
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
        public IReadOnlyList<IConsumerDefinition> ConsumerDefinitions { get; } = [.. consumers];

        public T GetPropertyOrDefault<T>(string key, T defaultValue) => Properties.GetOrDefault(key, defaultValue);
    }
}