using Portic.Consumer;
using System.Diagnostics.CodeAnalysis;

namespace Portic.Endpoint
{
    public interface IEndpointConfiguration
    {
        string Name { get; }
        IReadOnlyDictionary<string, IConsumerConfiguration> Consumers { get; }

        T GetPropertyOrDefault<T>(string key, T defaultValue);
        bool TryGetConsumerForMessage(string? messageName, [NotNullWhen(true)] out IConsumerConfiguration? consumer);
    }
}
