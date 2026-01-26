using Portic.Endpoint;

namespace Portic.Abstractions
{
    public interface IPorticConfiguration
    {
        IReadOnlyList<IEndpointConfiguration> Endpoints { get; }
        internal IReadOnlyList<Type> Middleware { get; }

        IMessageConfiguration GetMessageConfiguration<TMessage>();
    }
}
