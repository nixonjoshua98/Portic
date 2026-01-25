using Portic.Consumer;

namespace Portic.Abstractions
{
    public interface IPorticConfiguration
    {
        IReadOnlyList<IMessageConsumerConfiguration> Consumers { get; }
        IReadOnlyList<IMessageConfiguration> Messages { get; }

        IMessageConsumerConfiguration? GetConsumerForMessage(IMessageConfiguration messageConfiguration);
        IMessageConfiguration GetMessageConfiguration<TMessage>();
        IMessageConfiguration GetMessageConfiguration(string messageName);
    }
}
