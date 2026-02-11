using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portic.Consumers;
using Portic.Endpoints;
using Portic.Exceptions;
using Portic.Messages;
using Portic.Middleware;
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

        public IPorticConfigurator WithMaxRedeliveryAttempts(byte attempts)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(attempts, nameof(attempts));

            MaxRedeliveryAttempts = attempts;

            return this;
        }

        public IPorticConfigurator SetTransportDefinition<TTransport, TReceiveEndpointFactory>(ITransportDefinition transportDefinition)
            where TTransport : class, IMessageTransport
            where TReceiveEndpointFactory : class, IReceiveEndpointFactory
        {
            if (TransportDefinition is not null)
            {
                throw TransportAlreadyDefinedException.FromTransport(TransportDefinition, transportDefinition);
            }

            TransportDefinition = transportDefinition;

            // Transport
            Services.TryAddSingleton<TTransport>();
            Services.TryAddSingleton<IMessageTransport>(sp => sp.GetRequiredService<TTransport>());

            // Receive Endpoint Factory
            Services.TryAddSingleton<TReceiveEndpointFactory>();
            Services.TryAddSingleton<IReceiveEndpointFactory>(sp => sp.GetRequiredService<TReceiveEndpointFactory>());

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

            var messageDefinitions = _messageConfigurators
                .ToDictionary(x => x.Key, x => x.Value.ToDefinition());

            CommonDefinitionValidator.ValidateMessageDefinitions(messageDefinitions.Values);

            var consumers = _consumerBuilders.Values
                .Select(c => c.Build(messageDefinitions[c.MessageType]))
                .ToList();

            var middleware = _middleware
                .Select(m => new MiddlewareDefinition(m))
                .ToList();

            return new PorticConfiguration(
                messageDefinitions,
                BuildEndpointDefinitions(transport, consumers),
                middleware
            );
        }

        private IEnumerable<IEndpointDefinition> BuildEndpointDefinitions(ITransportDefinition transportDefinition, List<IConsumerDefinition> allConsumerDefinitions)
        {
            foreach (var (_, configurator) in _endpointConfigurators)
            {
                var endpointConsumers = allConsumerDefinitions
                    .Where(consumer => consumer.EndpointName == configurator.Name);

                var endpointDefinition = configurator.ToDefinition(this, endpointConsumers);

                CommonDefinitionValidator.ValidateSingleMessageConsumerEndpoint(endpointDefinition);

                yield return endpointDefinition;
            }
        }
    }
}