using Portic.Transport;

namespace Portic.Consumers
{
    public interface IConsumerContext
    {
        string MessageId { get; }
        byte DeliveryCount { get; }
        IServiceProvider Services { get; }
        byte MaxRedeliveryAttempts { get; }
        CancellationToken CancellationToken { get; }
        IConsumerDefinition ConsumerDefinition { get; }
        IMessageSettlement Settlement { get; }

        IConsumerContext WithServiceProvider(IServiceProvider serviceProvider);
    }

    public interface IConsumerContext<TMessage> : IConsumerContext
    {
        TMessage Message { get; }
    }
}
