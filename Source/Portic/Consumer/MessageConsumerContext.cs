using Portic.Transport;

namespace Portic.Consumer
{
    internal sealed class MessageConsumerContext<TMessage> : IConsumerContext<TMessage>
    {
        public TMessage Message { get; }
        public TimeSpan Latency { get; }
        public CancellationToken CancellationToken { get; }

        public MessageConsumerContext(
            DateTimeOffset timestamp,
            ITransportPayload<TMessage> payload,
            CancellationToken cancellationToken)
        {
            Message = payload.Message;
            Latency = timestamp - payload.PublishedAt;

            CancellationToken = cancellationToken;
        }
    }
}
