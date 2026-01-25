namespace Portic.Endpoint
{
    public interface IEndpointConfigurator
    {
        string Name { get; }

        IEndpointConfigurator SetProperty(string key, object value);
    }
}