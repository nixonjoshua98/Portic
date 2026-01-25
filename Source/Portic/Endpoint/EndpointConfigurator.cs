using Portic.Consumer;

namespace Portic.Endpoint
{
    internal sealed class EndpointConfigurator : IEndpointConfigurator
    {
        public string Name { get; private set; }

        public EndpointConfigurator(string endpointName)
        {
            Name = endpointName;
        }

        public IEndpointConfiguration Build(IEnumerable<IMessageConsumerConfiguration> consumers)
        {
            return new EndpointConfiguration(
                Name, 
                consumers
            );
        }
    }
}
