using Microsoft.Extensions.Logging;

namespace Portic.Transport.RabbitMQ.Logging
{
    internal static partial class LoggingExtensions
    {
        [LoggerMessage(Level = LogLevel.Debug, Message = "Message '{MessageName}' has been consumed from queue '{QueueName}'")]
        public static partial void LogMessageConsumed(this ILogger logger, string messageName, string queueName);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Queue '{QueueName}' has been bound to exchange '{ExchangeName}'")]
        public static partial void LogExchangeBoundToQueue(this ILogger logger, string queueName, string exchangeName);
    }
}
