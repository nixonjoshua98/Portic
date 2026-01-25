using Portic.Transport;

namespace Portic.Consumer
{
    public interface IMessageConsumerContextFactory
    {
        IConsumerContext<TMessage> CreateContext<TMessage>(ITransportPayload<TMessage> message, CancellationToken cancellationToken);
    }
}
