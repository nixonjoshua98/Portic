using Microsoft.Extensions.Logging;

namespace Portic.Middleware.Polly.Logging
{
    internal static partial class PollyMiddlewareLog
    {
        [LoggerMessage(Level = LogLevel.Error, Message = "Polly resilience pipeline failed for message '{MessageName}' after exhausting all policies")]
        public static partial void LogFailedResiliencePipeline(this ILogger logger, Exception exception, string messageName);
    }
}
