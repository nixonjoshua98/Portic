using Portic.Endpoints;
using Portic.Messages;
using Portic.Transport;

namespace Portic.Configuration
{
    public interface IPorticConfiguration
    {
        IReadOnlyList<IEndpointDefinition> Endpoints { get; }

        internal ITransportDefinition TransportDefinition { get; }
        internal IReadOnlyList<Type> Middleware { get; }

        IMessageDefinition GetMessageDefinition<TMessage>();
    }
}
