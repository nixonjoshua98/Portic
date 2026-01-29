namespace Portic.Transport
{
    public interface IMessageSettlement
    {
        Task CompleteAsync(CancellationToken cancellationToken);

        Task DeferAsync(CancellationToken cancellationToken);

        Task FaultAsync(Exception exception, CancellationToken cancellationToken);
    }
}
