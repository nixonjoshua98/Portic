namespace Portic.Consumers
{
    public interface IReceiveEndpoint : IDisposable
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
