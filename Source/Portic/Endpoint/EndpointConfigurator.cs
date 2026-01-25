using Portic.Consumer;
using Portic.Models;

namespace Portic.Endpoint
{
    internal sealed class EndpointConfigurator : IEndpointConfigurator
    {
        public string Name { get; private set; }

        public ICustomPropertyBag Properties { get; } = new CustomPropertyBag();

        public EndpointConfigurator(string endpointName)
        {
            Name = endpointName;
        }

        public void SetProperty(string key, object value)
        {
            Properties.SetProperty(key, value);
        }

        public IEndpointConfiguration Build(IEnumerable<IConsumerConfiguration> consumers)
        {
            return new EndpointConfiguration(
                Name, 
                consumers,
                Properties
            );
        }
    }
}
