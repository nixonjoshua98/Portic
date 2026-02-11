namespace Portic.Transport.InMemory.Transport
{
    internal sealed class InMemoryTransportDefinition : ITransportDefinition, IInMemoryTransportConfigurator
    {
        public string DisplayName { get; } = "InMemory";
    }
}