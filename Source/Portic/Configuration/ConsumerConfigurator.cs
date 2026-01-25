using Portic.Abstractions;
using Portic.Consumer;

namespace Portic.Configuration
{
    internal sealed class ConsumerConfigurator : IMessageConsumerBuilder
    {
        public Type ConsumerType { get; }
        public Type MessageType { get; }

        public ConsumerConfigurator(Type consumerType, Type messageConfigurator)
        {
            ConsumerType = consumerType;
            MessageType = messageConfigurator;
        }

        public IMessageConsumerConfiguration Build(IMessageConfiguration message)
        {
            return new ConsumerConfiguration(message, ConsumerType);
        }
    }
}