using Microsoft.Extensions.Logging;

namespace Portic.Middleware.Polly.Extensions
{
    internal static partial class LoggingExtensions
    {
        [LoggerMessage(Level = LogLevel.Error, Message = "Polly resilience pipeline failed for message '{MessageId}' after exhausting all policies")]
        public static partial void LogFailedResiliencePipeline(this ILogger logger, Exception exception, string messageId);

        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "Polly retry policy retrying message '{MessageId}' (Attempt: {AttemptNumber} of {MaxRetryAttempts}, Delay: {RetryDelay:F2}ms)"
        )]
        public static partial void LogRetryAttempt(this ILogger logger, string messageId, int attemptNumber, int maxRetryAttempts, double retryDelay);
    }
}
