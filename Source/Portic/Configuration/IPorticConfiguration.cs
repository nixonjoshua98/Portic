using Portic.Endpoints;
using Portic.Messages;
using Portic.Transport;
using System.Diagnostics.CodeAnalysis;

namespace Portic.Configuration
{
    public interface IPorticConfiguration
    {
        IReadOnlyList<IEndpointDefinition> Endpoints { get; }
        ITransportDefinition TransportDefinition { get; }
        internal IReadOnlyList<Type> Middleware { get; }

        IMessageDefinition GetMessageDefinition<TMessage>();

        internal bool TryGetEndpointByName(string name, [NotNullWhen(true)] out IEndpointDefinition? endpoint);
    }
}
