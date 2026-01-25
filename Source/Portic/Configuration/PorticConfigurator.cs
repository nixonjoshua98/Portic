using Microsoft.Extensions.DependencyInjection;
using Portic.Abstractions;
using Portic.Consumer;
using System.Collections.Concurrent;

namespace Portic.Configuration
{
    internal sealed class PorticConfigurator : IPorticConfigurator
    {
        public IServiceCollection Services { get; }

        private readonly ConcurrentDictionary<Type, MessageConfigurator> MessageConfigurators = [];

        public readonly ConcurrentDictionary<Type, ConsumerConfigurator> ConsumerBuilders = [];

        public PorticConfigurator(IServiceCollection services)
        {
            Services = services;
        }

        public IMessageConsumerBuilder ConfigureConsumer<TMessage, TMessageConsumer>()
        {
            var messageConfigurator = ConfigureMessage<TMessage>();

            return ConfigureConsumer<TMessageConsumer>(messageConfigurator.MessageType);
        }

        private ConsumerConfigurator ConfigureConsumer<TConsumer>(Type messageType)
        {
            var consumerType = typeof(TConsumer);

            return ConsumerBuilders.GetOrAdd(
                consumerType,
                _ => new ConsumerConfigurator(consumerType, messageType)
            );
        }

        public IMessageConfigurator ConfigureMessage<TMessage>()
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

            return new PorticConfiguration(
                consumerConfigurations,
                [.. messageConfigurators.Values]
            );
        }
    }
}