using Portic.Transport;

namespace Portic.Consumer
{
    public sealed record ConsumerExecutorContext<TMessage>(
        ITransportPayload<TMessage> Payload,
        IServiceProvider Services,
        IConsumerConfiguration Consumer
    );
}
