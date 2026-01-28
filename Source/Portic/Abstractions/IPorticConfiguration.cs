using Portic.Endpoint;
using System.Diagnostics.CodeAnalysis;

namespace Portic.Abstractions
{
    public interface IPorticConfiguration
    {
        IReadOnlyList<IEndpointConfiguration> Endpoints { get; }

        internal IReadOnlyList<Type> Middleware { get; }

        IMessageConfiguration GetMessageConfiguration<TMessage>();

        internal bool TryGetEndpointByName(string name, [NotNullWhen(true)] out IEndpointConfiguration? endpoint);
    }
}
