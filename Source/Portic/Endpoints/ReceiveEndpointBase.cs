namespace Portic.Endpoints
{
    public abstract class ReceiveEndpointBase : IReceiveEndpoint
    {
        protected bool _isDisposed;

        private CancellationTokenSource? _lifetimeSource = default;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _lifetimeSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            await RunAsync(_lifetimeSource.Token);
        }

        protected abstract Task RunAsync(CancellationToken cancellationToken);

        protected virtual void Dispose(bool disposing)
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
