using Portic.Configuration;
using Portic.Consumer;
using Portic.Models;

namespace Portic.Endpoint
{
    internal sealed class EndpointConfigurator : IEndpointConfigurator
    {
        private readonly CustomPropertyBag Properties = new();

        public string Name { get; private set; }

        public EndpointConfigurator(string endpointName)
        {
            Name = endpointName;
        }

        public IEndpointConfigurator SetProperty(string key, object value)
        {
            Properties.Set(key, value);

            return this;
        }

        public IEndpointConfiguration Build(PorticConfigurator configurator, IEnumerable<IConsumerConfiguration> consumers)
        {
            return new EndpointConfiguration(
                Name,
                consumers,
                Properties,
                configurator.MaxRedeliveryAttempts
            );
        }
    }
}
