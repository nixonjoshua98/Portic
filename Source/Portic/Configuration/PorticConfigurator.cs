using Microsoft.Extensions.DependencyInjection;
using Portic.Consumers;
using Portic.Endpoints;
using Portic.Exceptions;
using Portic.Messages;
using Portic.Models;
using Portic.Transport;
using Portic.Validation;
using System.Collections.Concurrent;

namespace Portic.Configuration
{
    internal sealed class PorticConfigurator(IServiceCollection services) : IPorticConfigurator
    {
        private readonly CustomPropertyBag _properties = new();

        private readonly ConcurrentDictionary<Type, MessageConfigurator> _messageConfigurators = [];

        private readonly ConcurrentDictionary<Type, ConsumerConfigurator> _consumerBuilders = [];

        private readonly ConcurrentDictionary<string, EndpointConfigurator> _endpointConfigurators = [];

        private readonly List<Type> _middleware = [];

        public IServiceCollection Services { get; } = services;
        private ITransportDefinition? TransportDefinition { get; set; }
        internal byte MaxRedeliveryAttempts { get; private set; } = 0;

        public IPorticConfigurator WithMessageRedeliveryCount(byte attempts)
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
            _middleware.Add(typeof(TMiddleware));

            return this;
        }

        public IPorticConfigurator SetProperty(string key, object value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

            _properties.Set(key, value);

            return this;
        }

        public bool HasProperty(string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

            return _properties.ContainsKey(key);
        }

        private ConsumerConfigurator GetConsumerConfigurator<TConsumer>(Type messageType)
        {
            var consumerType = typeof(TConsumer);

            return _consumerBuilders.GetOrAdd(
                consumerType,
                _ => new ConsumerConfigurator(this, consumerType, messageType)
            );
        }

        private EndpointConfigurator GetEndpointConfigurator(string endpointName)
        {
            return _endpointConfigurators.GetOrAdd(
                endpointName,
                _ => new EndpointConfigurator(endpointName)
            );
        }

        private MessageConfigurator GetMessageConfigurator<TMessage>()
        {
            var messageType = typeof(TMessage);

            return _messageConfigurators.GetOrAdd(
                messageType,
                _ => new MessageConfigurator(messageType)
            );
        }

        public IPorticConfiguration Build()
        {
            var transport = TransportDefinition ?? throw new TransportNotDefinedException();

            var messageDefinitions = _messageConfigurators.Values
                .Select(x => x.Build())
                .ToDictionary(x => x.MessageType);

            DefinitionValidator.ValidateMessageDefinitions(messageDefinitions.Values);

            var consumers = _consumerBuilders.Values
                .Select(c => c.Build(messageDefinitions[c.MessageType]))
                .ToList();

            var endpoints = _endpointConfigurators.Values
                .Select(endpoint => endpoint.Build(
                    this,
                    consumers.Where(consumer => consumer.EndpointName == endpoint.Name)
                ))
                .ToList();

            return new PorticConfiguration(
                messageDefinitions,
                endpoints,
                _middleware,
                transport
            );
        }
    }
}