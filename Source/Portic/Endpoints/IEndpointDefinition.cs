using Portic.Consumers;

namespace Portic.Endpoints
{
    public interface IEndpointDefinition
    {
        string Name { get; }
        IReadOnlyDictionary<string, IConsumerDefinition> Consumers { get; }
        byte MaxRedeliveryAttempts { get; }

        IConsumerDefinition GetConsumerDefinition(string? messageName);
        T GetPropertyOrDefault<T>(string key, T defaultValue);
    }
}