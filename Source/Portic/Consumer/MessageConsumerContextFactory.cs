using Portic.Transport;

namespace Portic.Consumer
{
    internal sealed class MessageConsumerContextFactory : IMessageConsumerContextFactory
    {
        public IMessageConsumerContext<TMessage> CreateContext<TMessage>(ITransportPayload<TMessage> payload, CancellationToken cancellationToken)
        {
            return new MessageConsumerContext<TMessage>(
                DateTimeOffset.UtcNow,
                payload,
                cancellationToken
            );
        }
    }
}