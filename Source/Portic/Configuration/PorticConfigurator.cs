using Microsoft.Extensions.DependencyInjection;
using Portic.Abstractions;
using Portic.Consumer;
using Portic.Endpoint;
using System.Collections.Concurrent;

namespace Portic.Configuration
{
    internal sealed class PorticConfigurator : IPorticConfigurator
    {
        public IServiceCollection Services { get; }

        private readonly ConcurrentDictionary<Type, MessageConfigurator> MessageConfigurators = [];

        private readonly ConcurrentDictionary<Type, MessageConsumerConfigurator> ConsumerBuilders = [];

        private readonly ConcurrentDictionary<string, EndpointConfigurator> EndpointConfigurators = [];

        public PorticConfigurator(IServiceCollection services)
        {
            Services = services;
        }

        public IMessageConfigurator ConfigureMessage<TMessage>()
        {
            return GetMessageConfigurator<TMessage>();
        }

        public IEndpointConfigurator ConfigureEndpoint(string endpointName)
        {
            return GetEndpointConfigurator(endpointName);
        }

        public IMessageConsumerConfigurator ConfigureConsumer<TMessage, TMessageConsumer>()
        {
            var message = ConfigureMessage<TMessage>();

            var consumer = GetConsumerConfigurator<TMessageConsumer>(message.MessageType);

            ConfigureEndpoint(consumer.EndpointName);

            return consumer;
        }

        private MessageConsumerConfigurator GetConsumerConfigurator<TConsumer>(Type messageType)
        {
            var consumerType = typeof(TConsumer);

            return ConsumerBuilders.GetOrAdd(
                consumerType,
                _ => new MessageConsumerConfigurator(this, consumerType, messageType)
            );
        }

        private EndpointConfigurator GetEndpointConfigurator(string endpointName)
        {
            return EndpointConfigurators.GetOrAdd(
                endpointName,
                _ => new EndpointConfigurator(endpointName)
            );
        }

        private MessageConfigurator GetMessageConfigurator<TMessage>()
        {
            var messageType = typeof(TMessage);

            return MessageConfigurators.GetOrAdd(
                messageType,
                _ => new MessageConfigurator(messageType)
            );
        }

        public IPorticConfiguration Build()
        {
            var messageConfigurators = MessageConfigurators
                .ToDictionary(x => x.Key, x => x.Value.Build());

            // Message is quaranteed to be configured before the consumer,
            // even if its only the defaults
            var consumerConfigurations = ConsumerBuilders.Values
                .Select(c => c.Build(messageConfigurators[c.MessageType]))
                .ToList();

            var endpoints = EndpointConfigurators.Values
                .Select(endpoint => endpoint.Build(
                    consumerConfigurations.Where(consumer => consumer.EndpointName == endpoint.Name)
                ))
                .ToList();

            return new PorticConfiguration(
                consumerConfigurations,
                [.. messageConfigurators.Values],
                endpoints
            );
        }
    }
}