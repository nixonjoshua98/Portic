using Portic.Consumers;

namespace Portic.Endpoints
{
    public interface IEndpointDefinition
    {
        string Name { get; }
        byte MaxRedeliveryAttempts { get; }
        IReadOnlyList<IConsumerDefinition> ConsumerDefinitions { get; }

        IConsumerDefinition GetConsumerDefinition(string? messageName);
        T GetPropertyOrDefault<T>(string key, T defaultValue);
    }
}