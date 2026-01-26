using Portic.Transport;

namespace Portic.Consumer
{
    internal sealed class ConsumerContext<TMessage>(
        ITransportPayload<TMessage> payload,
        IConsumerConfiguration consumerConfiguration,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken
    ) : IConsumerContext<TMessage>
    {
        public string MessageId { get; } = payload.MessageId;
        public string MessageName { get; } = consumerConfiguration.Message.Name;
        public TMessage Message { get; } = payload.Message;
        public CancellationToken CancellationToken { get; } = cancellationToken;
        public IServiceProvider Services { get; private set; } = serviceProvider;
        public IConsumerConfiguration Consumer { get; } = consumerConfiguration;

        public IConsumerContext WithServiceProvider(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));

            Services = serviceProvider;

            return this;
        }
    }
}
