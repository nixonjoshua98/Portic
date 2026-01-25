using Portic.Consumer;
using Portic.Endpoint;

namespace Portic.Abstractions
{
    public interface IPorticConfiguration
    {
        IReadOnlyList<IMessageConsumerConfiguration> Consumers { get; }
        IReadOnlyList<IMessageConfiguration> Messages { get; }
        IReadOnlyList<IEndpointConfiguration> Endpoints { get; }

        IMessageConsumerConfiguration? GetConsumerForMessage(IMessageConfiguration messageConfiguration);
        IMessageConfiguration GetMessageConfiguration<TMessage>();
        IMessageConfiguration GetMessageConfiguration(string messageName);
    }
}
