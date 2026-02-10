namespace Portic.Tests.Common.Helpers
{
    public sealed class TrackableMessageSource<T>
    {
        private readonly TaskCompletionSource<T> CompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<T> WaitAsync(CancellationToken cancellationToken) => CompletionSource.Task.WaitAsync(cancellationToken);

        public void SetResult(T result) => CompletionSource.TrySetResult(result);
    }
}
