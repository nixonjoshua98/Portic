using Portic.Transport;

namespace Portic.Consumer
{
    public interface IMessageConsumerContextFactory
    {
        IMessageConsumerContext<TMessage> CreateContext<TMessage>(ITransportPayload<TMessage> message, CancellationToken cancellationToken);
    }
}
