using Microsoft.Extensions.Logging;

namespace Portic.Logging
{
    internal static partial class PorticLog
    {
        [LoggerMessage(Level = LogLevel.Debug, Message = "Message '{MessageName}' has been consumed")]
        public static partial void LogMessageConsumed(this ILogger logger, string messageName);
    }
}
