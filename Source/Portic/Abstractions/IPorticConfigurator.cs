using Microsoft.Extensions.DependencyInjection;
using Portic.Consumer;
using Portic.Endpoint;

namespace Portic.Abstractions
{
    public interface IPorticConfigurator
    {
        IServiceCollection Services { get; }

        IConsumerConfigurator ConfigureConsumer<TMessage, TMessageConsumer>();
        IEndpointConfigurator ConfigureEndpoint(string endpointName);
        IMessageConfigurator ConfigureMessage<TMessage>();
        bool HasProperty(string key);
        IPorticConfigurator SetProperty(string key, object value);
        IPorticConfigurator Use<TMiddleware>() where TMiddleware : IConsumerMiddleware;
    }
}
