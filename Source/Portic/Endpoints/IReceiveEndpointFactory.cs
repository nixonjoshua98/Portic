namespace Portic.Endpoints
{
    public interface IReceiveEndpointFactory
    {
        Task<IReceiveEndpoint> CreateReceiveEndpointAsync(IEndpointDefinition endpointDefinition, CancellationToken cancellationToken);
    }
}
