namespace Portic.Transport
{
    public interface IMessageSettlement
    {
        Task CompleteAsync(CancellationToken cancellationToken);

        Task DeferAsync(Exception exception, CancellationToken cancellationToken);

        Task FaultAsync(Exception exception, CancellationToken cancellationToken);
    }
}
