using Microsoft.Extensions.Logging;

namespace Portic.Transport.InMemory.Extensions
{
    internal static partial class LoggingExtensions
    {
        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Message '{MessageId}' has faulted and been abandoned"
        )]
        public static partial void LogFaultedMessage(this ILogger logger, string messageId, Exception exception);
    }
}
