using Portic.Endpoints;
using Portic.Messages;
using System.Diagnostics.CodeAnalysis;

namespace Portic.Configuration
{
    public interface IPorticConfiguration
    {
        IReadOnlyList<IEndpointDefinition> Endpoints { get; }

        internal IReadOnlyList<Type> Middleware { get; }

        IMessageDefinition GetMessageConfiguration<TMessage>();

        internal bool TryGetEndpointByName(string name, [NotNullWhen(true)] out IEndpointDefinition? endpoint);
    }
}
