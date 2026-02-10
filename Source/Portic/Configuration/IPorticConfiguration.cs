using Portic.Endpoints;
using Portic.Messages;
using Portic.Transport;

namespace Portic.Configuration
{
    public interface IPorticConfiguration
    {
        IReadOnlyList<IEndpointDefinition> Endpoints { get; }
        IReadOnlyList<Type> Middleware { get; }

        IMessageDefinition GetMessageDefinition<TMessage>();
    }
}
