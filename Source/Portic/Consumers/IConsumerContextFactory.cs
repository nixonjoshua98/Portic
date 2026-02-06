using Portic.Transport;

namespace Portic.Consumers
{
    internal interface IConsumerContextFactory
    {
        ValueTask<IConsumerContext<TMessage>> CreateAsync<TMessage>(
            TransportMessageReceived<TMessage> message,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }
}