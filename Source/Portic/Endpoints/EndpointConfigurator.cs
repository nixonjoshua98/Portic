using Portic.Configuration;
using Portic.Consumers;
using Portic.Models;

namespace Portic.Endpoints
{
    internal sealed class EndpointConfigurator : IEndpointConfigurator
    {
        private readonly CustomPropertyBag _properties = new();

        public string Name { get; private set; }

        public EndpointConfigurator(string endpointName)
        {
            Name = endpointName;
        }

        public IEndpointConfigurator SetProperty(string key, object value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

            _properties.Set(key, value);

            return this;
        }

        public IEndpointDefinition ToDefinition(PorticConfigurator configurator, IEnumerable<IConsumerDefinition> consumers)
        {
            return new EndpointDefinition(
                Name,
                consumers,
                _properties,
                configurator.MaxRedeliveryAttempts
            );
        }
    }
}
