using Portic.Configuration;
using Portic.Consumers;
using Portic.Models;

namespace Portic.Endpoints
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

        public IEndpointDefinition Build(PorticConfigurator configurator, IEnumerable<IConsumerDefinition> consumers)
        {
            return new EndpointDefinition(
                Name,
                consumers,
                Properties,
                configurator.MaxRedeliveryAttempts
            );
        }
    }
}
