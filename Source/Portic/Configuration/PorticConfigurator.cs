using Microsoft.Extensions.DependencyInjection;
using Portic.Abstractions;
using Portic.Consumer;
using Portic.Endpoint;
using System.Collections.Concurrent;

namespace Portic.Configuration
{
    internal sealed class PorticConfigurator(IServiceCollection services) : IPorticConfigurator
    {
        public IServiceCollection Services { get; } = services;

        private readonly ConcurrentDictionary<Type, MessageConfigurator> MessageConfigurators = [];

        private readonly ConcurrentDictionary<Type, MessageConsumerConfigurator> ConsumerBuilders = [];

        private readonly ConcurrentDictionary<string, EndpointConfigurator> EndpointConfigurators = [];

        private readonly List<Type> Middleware = [];

        public IMessageConfigurator ConfigureMessage<TMessage>()
        {
            return GetMessageConfigurator<TMessage>();
        }

        public IEndpointConfigurator ConfigureEndpoint(string endpointName)
        {
            return GetEndpointConfigurator(endpointName);
        }

        public IConsumerConfigurator ConfigureConsumer<TMessage, TMessageConsumer>()
        {
            var message = ConfigureMessage<TMessage>();

            var consumer = GetConsumerConfigurator<TMessageConsumer>(message.MessageType);

            ConfigureEndpoint(consumer.EndpointName);

            return consumer;
        }

        public IPorticConfigurator Use<TMiddleware>() where TMiddleware : IConsumerMiddleware
        {
            Middleware.Add(typeof(TMiddleware));

            return this;
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

        private Dictionary<Type, IMessageConfiguration> CreateMessageConfigurations()
        {
            var duplicateMessageName = MessageConfigurators.Values
                .GroupBy(x => x.Name)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(duplicateMessageName))
            {
                throw new InvalidOperationException($"Duplicate message name detected: '{duplicateMessageName}'. Message names must be unique.");
            }

            return MessageConfigurators
                .ToDictionary(x => x.Key, x => x.Value.Build());
        }

        public IPorticConfiguration Build()
        {
            var messages = CreateMessageConfigurations();

            var consumers = ConsumerBuilders.Values
                .Select(c => c.Build(messages[c.MessageType]))
                .ToList();

            var endpoints = EndpointConfigurators.Values
                .Select(endpoint => endpoint.Build(
                    consumers.Where(consumer => consumer.EndpointName == endpoint.Name)
                ))
                .ToList();

            return new PorticConfiguration(
                messages,
                endpoints,
                Middleware
            );
        }
    }
}