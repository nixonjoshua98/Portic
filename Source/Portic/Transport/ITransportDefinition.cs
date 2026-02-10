using Portic.Endpoints;

namespace Portic.Transport
{
    public interface ITransportDefinition
    {
        string DisplayName { get; }

        void ValidateEndpoint(IEndpointDefinition endpointDefinition)
        {
            // Optional validation
        }
    }
}
