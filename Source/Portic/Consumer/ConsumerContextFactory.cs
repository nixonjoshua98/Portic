using Portic.Transport;

namespace Portic.Consumer
{
    internal sealed class ConsumerContextFactory : IConsumerContextFactory
    {
        public IConsumerContext<TMessage> CreateContext<TMessage>(ITransportPayload<TMessage> payload, CancellationToken cancellationToken)
        {
            return new ConsumerContext<TMessage>(
                payload,
                cancellationToken
            );
        }
    }
}