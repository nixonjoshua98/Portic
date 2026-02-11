using Portic.Consumers;
using Portic.Transport.InMemory.Consumers;
using Portic.Transport.InMemory.Topology;

namespace Portic.Transport.InMemory.Endpoints
{
    internal sealed class InMemoryReceiveEndpoint(InMemoryTransport _transport, InMemoryConsumerExecutor _consumerExecutor) : IReceiveEndpoint
    {
        private bool _isDisposed;

        private CancellationTokenSource? _lifetimeSource;

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _lifetimeSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            await foreach (var message in _transport.GetMessagesAsync(_lifetimeSource.Token))
            {
                await _consumerExecutor.ExecuteAsync(message, _lifetimeSource.Token);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (_lifetimeSource is { IsCancellationRequested: false })
                    {
                        _lifetimeSource.Cancel();
                    }

                    _lifetimeSource?.Dispose();
                }

                _lifetimeSource = null;
                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}