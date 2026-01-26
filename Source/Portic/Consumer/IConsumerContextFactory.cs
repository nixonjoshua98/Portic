using Portic.Transport;

namespace Portic.Consumer
{
    public interface IConsumerContextFactory
    {
        IConsumerContext<TMessage> CreateContext<TMessage>(ITransportPayload<TMessage> message, CancellationToken cancellationToken);
    }
}
