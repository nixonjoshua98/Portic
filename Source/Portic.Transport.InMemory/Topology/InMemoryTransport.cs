using Portic.Configuration;
using Portic.Transport.InMemory.Extensions;
using System.Collections.Concurrent;

namespace Portic.Transport.InMemory.Topology
{
    internal sealed class InMemoryTransport(IPorticConfiguration _configuration) : IInMemoryTransport
    {
        private readonly ConcurrentQueue<InMemoryQueuedMessage> MessageQueue = new();

        public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
        {
            var messageDefinition = _configuration.GetMessageDefinition<TMessage>();
            var endpointConsumers = _configuration.GetConsumers(messageDefinition);

            foreach (var (endpointDefinition, consumerDefinition) in endpointConsumers)
            {
                var queued = new InMemoryQueuedMessage(
                    Guid.CreateVersion7().ToString(),
                    message,
                    0,
                    messageDefinition,
                    consumerDefinition,
                    endpointDefinition
                );

                MessageQueue.Enqueue(queued);
            }

            return Task.CompletedTask;
        }

        public Task PublishDeferredAsync(InMemoryQueuedMessage message, Exception exception, CancellationToken cancellationToken)
        {
            var queued = message with
            {
                DeliveryCount = Convert.ToByte(message.DeliveryCount + 1)
            };

            MessageQueue.Enqueue(queued);

            return Task.CompletedTask;
        }

        public async Task<InMemoryQueuedMessage> WaitForMessageAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (MessageQueue.TryDequeue(out var result))
                {
                    return result;
                }

                await Task.Delay(10, cancellationToken); // We can probably use a cts?
            }
        }
    }
}