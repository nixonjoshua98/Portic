using Microsoft.Extensions.DependencyInjection;
using Portic.Consumers;
using Portic.Endpoints;
using Portic.Exceptions;
using Portic.Messages;
using Portic.Models;
using Portic.Transport;
using System.Collections.Concurrent;

namespace Portic.Configuration
{
    internal sealed class PorticConfigurator(IServiceCollection services) : IPorticConfigurator
    {
        private readonly CustomPropertyBag Properties = new();

        public IServiceCollection Services { get; } = services;
        private ITransportDefinition? TransportDefinition { get; set; }


        private readonly ConcurrentDictionary<Type, MessageConfigurator> MessageConfigurators = [];

        private readonly ConcurrentDictionary<Type, ConsumerConfigurator> ConsumerBuilders = [];

        private readonly ConcurrentDictionary<string, EndpointConfigurator> EndpointConfigurators = [];

        private readonly List<Type> Middleware = [];

        internal byte MaxRedeliveryAttempts { get; private set; } = 0;

        public IPorticConfigurator SetMaxRedeliveryAttempts(byte attempts)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(attempts, nameof(attempts));

            MaxRedeliveryAttempts = attempts;

            return this;
        }

        public IPorticConfigurator SetTransportDefinition(ITransportDefinition transportDefinition)
        {
            if (TransportDefinition is not null)
            {
                throw TransportAlreadyDefinedException.FromTransport(TransportDefinition, transportDefinition);
            }

            TransportDefinition = transportDefinition;

            return this;
        }

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

        public IPorticConfigurator SetProperty(string key, object value)
        {
            Properties.Set(key, value);

            return this;
        }

        public bool HasProperty(string key)
        {
            return Properties.ContainsKey(key);
        }

        private ConsumerConfigurator GetConsumerConfigurator<TConsumer>(Type messageType)
        {
            var consumerType = typeof(TConsumer);

            return ConsumerBuilders.GetOrAdd(
                consumerType,
                _ => new ConsumerConfigurator(this, consumerType, messageType)
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

        private Dictionary<Type, IMessageDefinition> CreateMessageConfigurations()
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
                    this,
                    consumers.Where(consumer => consumer.EndpointName == endpoint.Name)
                ))
                .ToList();

            return new PorticConfiguration(
                messages,
                endpoints,
                Middleware,
                TransportDefinition ?? throw TransportNotDefinedException.Create()
            );
        }
    }
}