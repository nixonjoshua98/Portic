using Portic.Consumer;
using Portic.Models;
using System.Diagnostics.CodeAnalysis;

namespace Portic.Endpoint
{
    public sealed class EndpointConfiguration(
        string name,
        IEnumerable<IConsumerConfiguration> consumers,
        IReadOnlyCustomPropertyBag properties
    ) : IEndpointConfiguration
    {
        private readonly IReadOnlyCustomPropertyBag Properties = properties;

        public string Name { get; } = name;
        public IReadOnlyDictionary<string, IConsumerConfiguration> Consumers { get; } = consumers.ToDictionary(x => x.Message.Name);

        public T GetPropertyOrDefault<T>(string key, T defaultValue) => Properties.GetOrDefault(key, defaultValue);

        public bool TryGetConsumerForMessage(string? messageName, [NotNullWhen(true)] out IConsumerConfiguration? consumer)
        {
            if (string.IsNullOrEmpty(messageName))
            {
                consumer = null;

                return false;
            }

            return Consumers.TryGetValue(messageName, out consumer);
        }
    }
}