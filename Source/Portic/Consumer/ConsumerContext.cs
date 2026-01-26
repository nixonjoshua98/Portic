using Portic.Transport;

namespace Portic.Consumer
{
    internal sealed class ConsumerContext<TMessage>(
        ITransportPayload<TMessage> payload,
        CancellationToken cancellationToken
    ) : IConsumerContext<TMessage>
    {
        public TMessage Message { get; } = payload.Message;
        public CancellationToken CancellationToken { get; } = cancellationToken;

        object IConsumerContext.Message => Message!;
    }
}
