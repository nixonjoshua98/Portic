namespace Portic.Endpoints
{
    public abstract class ReceiveEndpointBase : IReceiveEndpoint
    {
        protected bool _isDisposed;

        private CancellationTokenSource? _lifetimeSource;

        private readonly TaskCompletionSource _cancelledSource = new();
        private readonly TaskCompletionSource _completionSource = new();

        protected Task CompletedTask => _completionSource.Task;

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _lifetimeSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            await StartAsync(_lifetimeSource.Token);
        }

        protected void SetCompleted()
        {
            _completionSource.TrySetResult();
        }

        protected abstract Task StartAsync(CancellationToken cancellationToken);

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
