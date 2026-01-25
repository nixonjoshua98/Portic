using Portic.Abstractions;
using Portic.Consumer;

namespace Portic.Configuration
{
    internal sealed class ConsumerConfiguration : IMessageConsumerConfiguration
    {
        public IMessageConfiguration Message { get; }
        public Type ConsumerType { get; }

        public ConsumerConfiguration(IMessageConfiguration message, Type consumer)
        {
            Message = message;
            ConsumerType = consumer;
        }

        public string GetQueueName() => ConsumerType.FullName!;
    }
}
