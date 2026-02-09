namespace Portic.Transport.InMemory.Topology
{

    internal sealed class InMemoryTransportDefinition : ITransportDefinition, IInMemoryTransportConfigurator
    {
        public string DisplayName { get; } = "InMemory";

        public ITransportDefinition ToDefinition()
        {
            return this;
        }
    }
}