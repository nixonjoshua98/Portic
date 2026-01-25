namespace Portic.Consumer
{
    public interface IMessageConsumer<TMessage>
    {
        ValueTask ConsumeAsync(IMessageConsumerContext<TMessage> context);
    }
}
