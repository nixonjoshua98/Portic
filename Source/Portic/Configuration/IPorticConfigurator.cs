using Microsoft.Extensions.DependencyInjection;
using Portic.Consumers;
using Portic.Endpoints;
using Portic.Messages;
using Portic.Middleware;
using Portic.Transport;

namespace Portic.Configuration
{
    public interface IPorticConfigurator
    {
        /// <summary>
        /// Gets the collection of service descriptors for dependency injection configuration.
        /// </summary>
        /// <remarks>Use this property to register application services and configure dependency injection
        /// during application startup. Modifications to the collection affect the services available throughout the
        /// application's lifetime.</remarks>
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

        /// <summary>
        /// Determines whether a property with the specified key exists.
        /// </summary>
        /// <param name="key">The key of the property to locate. Cannot be null.</param>
        /// <returns>true if a property with the specified key exists; otherwise, false.</returns>
        bool HasProperty(string key);

        /// <summary>
        /// Sets the maximum number of redelivery attempts for failed messages.
        /// </summary>
        /// <param name="attempts">The maximum number of times a message will be redelivered before it is considered undeliverable. Must be
        /// greater than zero.</param>
        /// <returns>The current configurator instance for method chaining.</returns>
        IPorticConfigurator WithMaxRedeliveryAttempts(byte attempts);

        /// <summary>
        /// Sets a configuration property with the specified key and value.
        /// </summary>
        /// <param name="key">The name of the property to set. Cannot be null or empty.</param>
        /// <param name="value">The value to assign to the property. Can be null if the property supports null values.</param>
        /// <returns>The current configurator instance with the updated property, enabling method chaining.</returns>
        IPorticConfigurator SetProperty(string key, object value);

        /// <summary>
        /// Adds the specified consumer middleware type to the processing pipeline.
        /// </summary>
        /// <remarks>Middleware is invoked in the order it is added. Use this method to customize message
        /// processing by inserting custom logic into the consumer pipeline.</remarks>
        /// <typeparam name="TMiddleware">The type of middleware to add. Must implement the IConsumerMiddleware interface.</typeparam>
        /// <returns>The current configurator instance, enabling method chaining.</returns>
        IPorticConfigurator Use<TMiddleware>() where TMiddleware : IConsumerMiddleware;

        /// <summary>
        /// Registers a transport definition for the specified transport and receive endpoint factory types.
        /// </summary>
        /// <typeparam name="TTransport">The type of the message transport to associate with the transport definition. Must implement the
        /// IMessageTransport interface.</typeparam>
        /// <typeparam name="TReceiveEndpointFactory">The type of the receive endpoint factory to use with the transport. Must implement the
        /// IReceiveEndpointFactory interface.</typeparam>
        /// <param name="transportDefinition">The transport definition to register. Cannot be null.</param>
        /// <returns>The current configurator instance, allowing for method chaining.</returns>
        IPorticConfigurator SetTransportDefinition<TTransport, TReceiveEndpointFactory>(ITransportDefinition transportDefinition)
            where TTransport : class, IMessageTransport
            where TReceiveEndpointFactory : class, IReceiveEndpointFactory;
    }
}
