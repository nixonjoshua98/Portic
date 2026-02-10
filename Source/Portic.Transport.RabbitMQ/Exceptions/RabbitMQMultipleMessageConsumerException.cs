using Portic.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portic.Transport.RabbitMQ.Exceptions
{
    public sealed class RabbitMQMultipleMessageConsumerException : PorticException
    {
        internal RabbitMQMultipleMessageConsumerException(string endpointName, Type messageType) : 
            base($"Endpoint '{endpointName}' has been configured with multiple consumers for the same message type '{messageType.Name}'")
        {

        }
    }
}
