using Microsoft.Extensions.Logging;

namespace Portic.Transport.RabbitMQ.Logging
{
    internal static partial class LoggingExtensions
    {
        [LoggerMessage(Level = LogLevel.Debug, Message = "Queue '{QueueName}' has been bound to exchange '{ExchangeName}'")]
        public static partial void LogExchangeBoundToQueue(this ILogger logger, string queueName, string exchangeName);

        [LoggerMessage(Level = LogLevel.Information, Message = "Message '{MessageId}' has been successfully redelivered (Delivery: {DeliveryCount} of {MaxRedeliveryAttempts})")]
        public static partial void LogSuccessfulRedelivery(this ILogger logger, string? messageId, int deliveryCount, int maxRedeliveryAttempts);
    }
}
