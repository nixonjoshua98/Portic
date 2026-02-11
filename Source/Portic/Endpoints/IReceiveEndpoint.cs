namespace Portic.Endpoints
{
    public interface IReceiveEndpoint : IDisposable
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
