namespace Portic.Endpoints
{
    public interface IReceiveEndpoint : IDisposable
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
