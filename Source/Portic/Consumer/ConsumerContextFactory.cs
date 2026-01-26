namespace Portic.Consumer
{
    internal sealed class ConsumerContextFactory : IConsumerContextFactory
    {
        public IConsumerContext<TMessage> CreateContext<TMessage>(ConsumerExecutorContext<TMessage> context, CancellationToken cancellationToken)
        {
            return new ConsumerContext<TMessage>(
                context.Payload,
                context.Consumer,
                context.Services,
                cancellationToken
            );
        }
    }
}