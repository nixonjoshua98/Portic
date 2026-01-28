using Portic.Consumer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal interface IRabbitMQTransport : IMessageTransport
    {
        Task RePublishAsync<TMessage>(IConsumerContext<TMessage> context, CancellationToken cancellationToken);
    }
}
