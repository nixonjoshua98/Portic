using Microsoft.Extensions.Logging;

namespace Portic.Middleware.Polly.Extensions
{
    internal static partial class LoggingExtensions
    {
        [LoggerMessage(Level = LogLevel.Error, Message = "Polly resilience pipeline failed for message '{MessageName}' after exhausting all policies")]
        public static partial void LogFailedResiliencePipeline(this ILogger logger, Exception exception, string messageName);

        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "Polly retry policy retrying message '{MessageId}' (Name: {messageName}, Attempt: {AttemptNumber} of {MaxRetryAttempts}, Delay: {RetryDelay:F2}ms)"
        )]
        public static partial void LogRetryAttempt(this ILogger logger, string messageId, string messageName, int attemptNumber, int maxRetryAttempts, double retryDelay);
    }
}
