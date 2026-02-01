namespace Portic.Transport
{
    public interface IMessageTransport
    {
        /// <summary>
        /// Publishes a message asynchronously to the configured transport.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to publish.</typeparam>
        /// <param name="message">The message to publish.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous publish operation.</returns>
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
    }
}