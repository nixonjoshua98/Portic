using Portic.Endpoints;
using Portic.Messages;
using Portic.Middleware;

namespace Portic.Configuration
{
    public interface IPorticConfiguration
    {
        IReadOnlyList<IEndpointDefinition> Endpoints { get; }

        IReadOnlyList<IMiddlewareDefinition> Middleware { get; }

        IMessageDefinition GetMessageDefinition<TMessage>();
    }
}
