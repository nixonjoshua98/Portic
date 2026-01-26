namespace Portic.Consumer
{
    public sealed class ConsumerContext<TMessage>(
        string messageId,
        TMessage message,
        byte deliveryCount,
        IServiceProvider serviceProvider,
        IConsumerConfiguration consumer,
        CancellationToken cancellationToken
    ) : IConsumerContext<TMessage>
    {
        public string MessageId { get; } = messageId;
        public string MessageName { get; } = consumer.Message.Name;
        public TMessage Message { get; } = message;
        public CancellationToken CancellationToken { get; } = cancellationToken;
        public IServiceProvider Services { get; private set; } = serviceProvider;
        public IConsumerConfiguration Consumer { get; } = consumer;
        public byte DeliveryCount { get; } = deliveryCount;

        public IConsumerContext WithServiceProvider(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));

            Services = serviceProvider;

            return this;
        }
    }
}
