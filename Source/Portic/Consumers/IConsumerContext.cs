using Portic.Messages;

namespace Portic.Consumers
{
    public interface IConsumerContext
    {
        string MessageId { get; }
        string MessageName { get; }
        byte DeliveryCount { get; }
        IServiceProvider Services { get; }
        byte MaxRedeliveryAttempts { get; }
        CancellationToken CancellationToken { get; }
        IMessageDefinition MessageConfiguration { get; }
        IConsumerDefinition ConsumerConfiguration { get; }

        IConsumerContext WithServiceProvider(IServiceProvider serviceProvider);
    }

    public interface IConsumerContext<TMessage> : IConsumerContext
    {
        TMessage Message { get; }
    }
}
