
namespace Portic.Consumer
{
    public interface IConsumerContextFactory
    {
        ValueTask<IConsumerContext<TMessage>> CreateAsync<TMessage>(
            TransportMessageReceived<TMessage> message, 
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }
}