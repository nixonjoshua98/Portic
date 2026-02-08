using Microsoft.Extensions.Logging;

namespace Portic.Logging
{
    internal static partial class LoggingExtensions
    {
        [LoggerMessage(Level = LogLevel.Debug, Message = "Message '{MessageId}' has been consumed.")]
        public static partial void LogMessageConsumed(this ILogger logger, string messageId);

        [LoggerMessage(Level = LogLevel.Error, Message = "Message '{MessageId}' has faulted.")]
        public static partial void LogFaultedMessage(this ILogger logger, string messageId, Exception exception);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Message '{MessageId}' has been deferred (Delivery: {DeliveryCount} of {MaxRedeliveryAttempts})")]
        public static partial void LogDeferredMessage(this ILogger logger, string messageId, Exception exception, int deliveryCount, int maxRedeliveryAttempts);
    }
}
