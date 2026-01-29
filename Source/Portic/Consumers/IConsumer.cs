namespace Portic.Consumers
{
    public interface IConsumer<TMessage>
    {
        ValueTask ConsumeAsync(IConsumerContext<TMessage> context);
    }
}
