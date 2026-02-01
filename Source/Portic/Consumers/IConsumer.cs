namespace Portic.Consumers
{
    /// <summary>
    /// Defines a consumer that processes messages of type <typeparamref name="TMessage"/>.
    /// </summary>
    /// <typeparam name="TMessage">The type of message this consumer processes.</typeparam>
    public interface IConsumer<TMessage>
    {
        /// <summary>
        /// Consumes a message asynchronously.
        /// </summary>
        /// <param name="context">The context containing the message and execution details.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask ConsumeAsync(IConsumerContext<TMessage> context);
    }
}
