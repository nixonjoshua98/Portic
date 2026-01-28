
namespace Portic.Consumer
{
    internal interface IConsumerContextFactory
    {
        ValueTask<IConsumerContext<TMessage>> CreateAsync<TMessage>(
            TransportMessageReceived<TMessage> message,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }
}