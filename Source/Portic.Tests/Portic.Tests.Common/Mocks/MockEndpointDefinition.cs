using Portic.Consumers;
using Portic.Endpoints;

namespace Portic.Tests.Common.Mocks
{
    public sealed class MockEndpointDefinition(
        string name,
        byte maxRedeliveryAttempts,
        IEnumerable<IConsumerDefinition>? consumerDefinitions = null
    ) : IEndpointDefinition
    {
        public string Name { get; } = name;
        public byte MaxRedeliveryAttempts { get; } = maxRedeliveryAttempts;
        public IReadOnlyList<IConsumerDefinition> ConsumerDefinitions { get; } = consumerDefinitions is null ? [] : [.. consumerDefinitions];

        public T GetPropertyOrDefault<T>(string key, T defaultValue)
        {
            throw new NotImplementedException();
        }
    }
}
