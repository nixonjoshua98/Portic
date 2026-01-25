using Microsoft.Extensions.DependencyInjection;
using Portic.Consumer;
using Portic.Endpoint;

namespace Portic.Abstractions
{
    public interface IPorticConfigurator
    {
        IServiceCollection Services { get; }

        IMessageConsumerConfigurator ConfigureConsumer<TMessage, TMessageConsumer>();
        IEndpointConfigurator ConfigureEndpoint(string endpointName);
        IMessageConfigurator ConfigureMessage<TMessage>();
    }
}
