using Portic.Consumer;
using System.Diagnostics.CodeAnalysis;

namespace Portic.Endpoint
{
    public sealed class EndpointConfiguration(
        string name,
        IEnumerable<IMessageConsumerConfiguration> consumers
    ) : IEndpointConfiguration
    {
        public string Name { get; } = name;
        public IReadOnlyDictionary<string, IMessageConsumerConfiguration> Consumers { get; } = consumers.ToDictionary(x => x.Message.GetName());

        public bool TryGetConsumerForMessage(string messageName, [NotNullWhen(true)] out IMessageConsumerConfiguration? consumer)
        {
            return Consumers.TryGetValue(messageName, out consumer);
        }
    }
}
