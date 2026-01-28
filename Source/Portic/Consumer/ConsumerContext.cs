using Portic.Abstractions;
using Portic.Endpoint;

namespace Portic.Consumer
{
    internal sealed class ConsumerContext<TMessage>(
        string messageId,
        TMessage message,
        byte deliveryCount,
        IServiceProvider serviceProvider,
        IConsumerConfiguration consumer,
        IEndpointConfiguration endpoint,
        CancellationToken cancellationToken
    ) : IConsumerContext<TMessage>
    {
        public string MessageId { get; } = messageId;
        public string MessageName { get; } = consumer.Message.Name;
        public TMessage Message { get; } = message;
        public IMessageConfiguration MessageConfiguration { get; } = consumer.Message;
        public CancellationToken CancellationToken { get; } = cancellationToken;
        public IServiceProvider Services { get; private set; } = serviceProvider;
        public IConsumerConfiguration ConsumerConfiguration { get; } = consumer;
        public byte DeliveryCount { get; } = deliveryCount;
        public byte MaxRedeliveryAttempts { get; } = endpoint.MaxRedeliveryAttempts;

        public IConsumerContext WithServiceProvider(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));

            Services = serviceProvider;

            return this;
        }
    }
}
