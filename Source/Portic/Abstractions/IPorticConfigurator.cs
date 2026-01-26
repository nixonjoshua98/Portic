using Microsoft.Extensions.DependencyInjection;
using Portic.Consumer;
using Portic.Endpoint;

namespace Portic.Abstractions
{
    public interface IPorticConfigurator
    {
        IServiceCollection Services { get; }

        /// <summary>
        /// Configures a consumer for the specified message type and returns a configurator for further customization.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be consumed. This type determines the messages that the consumer will handle.</typeparam>
        /// <typeparam name="TMessageConsumer">The type of the consumer that processes messages of type <typeparamref name="TMessage"/></typeparam>
        /// <returns>An <see cref="IConsumerConfigurator"/> instance that can be used to further configure the consumer.</returns>
        IConsumerConfigurator ConfigureConsumer<TMessage, TMessageConsumer>();

        /// <summary>
        /// Configures the endpoint with the specified name and returns an endpoint configurator for further
        /// customization.
        /// </summary>
        /// <param name="endpointName">The name of the endpoint to configure.</param>
        /// <returns>An <see cref="IEndpointConfigurator"/> instance for configuring the specified endpoint.</returns>
        IEndpointConfigurator ConfigureEndpoint(string endpointName);

        /// <summary>
        /// Configures the pipeline for messages of the specified type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to configure in the pipeline.</typeparam>
        /// <returns>An <see cref="IMessageConfigurator"/> that can be used to further configure the message pipeline for the
        /// specified message type.</returns>
        IMessageConfigurator ConfigureMessage<TMessage>();

        bool HasProperty(string key);

        IPorticConfigurator SetProperty(string key, object value);

        /// <summary>
        /// Adds the specified consumer middleware type to the processing pipeline.
        /// </summary>
        /// <remarks>Middleware is invoked in the order it is added. Use this method to customize message
        /// processing by inserting custom logic into the consumer pipeline.</remarks>
        /// <typeparam name="TMiddleware">The type of middleware to add. Must implement the IConsumerMiddleware interface.</typeparam>
        /// <returns>The current configurator instance, enabling method chaining.</returns>
        IPorticConfigurator Use<TMiddleware>() where TMiddleware : IConsumerMiddleware;
    }
}
