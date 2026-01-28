namespace Portic.Consumer
{
    internal sealed class ConsumerContextFactory : IConsumerContextFactory
    {
        public ValueTask<IConsumerContext<TMessage>> CreateAsync<TMessage>(
            TransportMessageReceived<TMessage> message,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            var context = new ConsumerContext<TMessage>(
                message.MessageId,
                message.Message,
                message.DeliveryCount,
                serviceProvider,
                message.ConsumerConfiguration,
                message.EndpointConfiguration,
                cancellationToken
            );

            return ValueTask.FromResult<IConsumerContext<TMessage>>(context);
        }
    }
}

