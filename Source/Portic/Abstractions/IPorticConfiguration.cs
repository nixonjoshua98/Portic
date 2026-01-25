using Portic.Consumer;
using Portic.Endpoint;

namespace Portic.Abstractions
{
    public interface IPorticConfiguration
    {
        IReadOnlyList<IEndpointConfiguration> Endpoints { get; }

        IMessageConfiguration GetMessageConfiguration<TMessage>();
    }
}
