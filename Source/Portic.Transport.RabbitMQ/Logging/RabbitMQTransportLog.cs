using Microsoft.Extensions.Logging;

namespace Portic.Transport.RabbitMQ.Logging
{
    internal static partial class RabbitMQTransportLog
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Consumed message '{MessageName}' in {ElapsedMs:F2}ms.")]
        public static partial void LogMessageConsumed(ILogger logger, string messageName, long elapsedMs);
    }
}
