using Portic.Consumers;

namespace Portic.Endpoints
{
    public interface IEndpointDefinition
    {
        string Name { get; }
        byte MaxRedeliveryAttempts { get; }
        IReadOnlyList<IConsumerDefinition> ConsumerDefinitions { get; }
        T GetPropertyOrDefault<T>(string key, T defaultValue);
    }
}