using Portic.Consumer;

namespace Portic.Endpoint
{
    public interface IEndpointConfiguration
    {
        string Name { get; }
        IReadOnlyDictionary<string, IConsumerConfiguration> Consumers { get; }
        byte MaxRedeliveryAttempts { get; }

        IConsumerConfiguration GetConsumerConfiguration(string? messageName);
        T GetPropertyOrDefault<T>(string key, T defaultValue);
    }
}