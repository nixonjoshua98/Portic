using Portic.Models;

namespace Portic.Endpoint
{
    public interface IEndpointConfigurator
    {
        string Name { get; }

        void SetProperty(string key, object value);
    }
}