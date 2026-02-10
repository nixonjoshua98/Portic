using Portic.Configuration;
using Portic.Transport.InMemory.Extensions;
using System.Threading.Channels;

namespace Portic.Transport.InMemory.Topology
{
    internal sealed class InMemoryTransport(IPorticConfiguration _configuration) : IInMemoryTransport
    {
        private readonly Channel<InMemoryQueuedMessage> _messageChannel = Channel.CreateUnbounded<InMemoryQueuedMessage>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false
        });

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
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

                await _messageChannel.Writer.WriteAsync(queued, cancellationToken);
            }
        }

        public async Task PublishDeferredAsync(InMemoryQueuedMessage message, Exception exception, CancellationToken cancellationToken)
        {
            var queued = message with
            {
                DeliveryCount = Convert.ToByte(message.DeliveryCount + 1)
            };

            await _messageChannel.Writer.WriteAsync(queued, cancellationToken);
        }

        public async Task<InMemoryQueuedMessage> WaitForMessageAsync(CancellationToken cancellationToken)
        {
            return await _messageChannel.Reader.ReadAsync(cancellationToken);
        }

        public IAsyncEnumerable<InMemoryQueuedMessage> GetMessagesAsync(CancellationToken cancellationToken)
        {
            return _messageChannel.Reader.ReadAllAsync(cancellationToken);
        }
    }
}