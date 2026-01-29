namespace Portic.Endpoints
{
    public interface IEndpointConfigurator
    {
        string Name { get; }

        IEndpointConfigurator SetProperty(string key, object value);
    }
}