using Microsoft.Extensions.DependencyInjection;
using Portic.Consumer;

namespace Portic.Abstractions
{
    public interface IPorticConfigurator
    {
        IServiceCollection Services { get; }

        IMessageConsumerBuilder ConfigureConsumer<TMessage, TMessageConsumer>();

        IMessageConfigurator ConfigureMessage<TMessage>();
    }
}
