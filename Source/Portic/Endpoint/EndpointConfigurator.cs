using Portic.Consumer;
using Portic.Models;

namespace Portic.Endpoint
{
    internal sealed class EndpointConfigurator : IEndpointConfigurator
    {
        private readonly CustomPropertyBag Properties = new CustomPropertyBag();

        public string Name { get; private set; }

        public EndpointConfigurator(string endpointName)
        {
            Name = endpointName;
        }

        public IEndpointConfigurator SetProperty(string key, object value)
        {
            Properties.SetProperty(key, value);

            return this;
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
