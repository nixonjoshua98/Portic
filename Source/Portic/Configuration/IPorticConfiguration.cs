using Portic.Endpoints;
using Portic.Messages;

namespace Portic.Configuration
{
    public interface IPorticConfiguration
    {
        IReadOnlyList<IEndpointDefinition> Endpoints { get; }
        IReadOnlyList<Type> Middleware { get; }

        IMessageDefinition GetMessageDefinition<TMessage>();
    }
}
