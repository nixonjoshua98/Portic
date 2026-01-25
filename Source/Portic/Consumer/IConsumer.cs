namespace Portic.Consumer
{
    public interface IConsumer<TMessage>
    {
        ValueTask ConsumeAsync(IConsumerContext<TMessage> context);
    }
}
