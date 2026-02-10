using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portic.Transport.RabbitMQ.Extensions
{
    internal static partial class LoggingExtensions
    {
        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Queue '{QueueName}' has been bound to exchange '{ExchangeName}'"
        )]
        public static partial void LogQueueBinding(this ILogger logger, string exchangeName, string queueName);
    }
}
