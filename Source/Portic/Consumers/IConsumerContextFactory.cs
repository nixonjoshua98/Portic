using Portic.Transport;

namespace Portic.Consumers
{
    internal interface IConsumerContextFactory
    {
        ValueTask<IConsumerContext<TMessage>> CreateAsync<TMessage>(
            ITransportMessageReceived<TMessage> message,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }
}