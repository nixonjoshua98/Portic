using Portic.Abstractions;

namespace Portic.Consumer
{
    public interface IConsumerContext
    {
        string MessageId { get; }
        string MessageName { get; }
        byte DeliveryCount { get; }
        IServiceProvider Services { get; }
        byte MaxRedeliveryAttempts { get; }
        CancellationToken CancellationToken { get; }
        IMessageConfiguration MessageConfiguration { get; }
        IConsumerConfiguration ConsumerConfiguration { get; }

        IConsumerContext WithServiceProvider(IServiceProvider serviceProvider);
    }

    public interface IConsumerContext<TMessage> : IConsumerContext
    {
        TMessage Message { get; }
    }
}
