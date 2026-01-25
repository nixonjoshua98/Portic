using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portic.Transport.RabbitMQ.Logging
{
    internal static partial class RabbitMQTransportLog
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Consumed message '{MessageName}' in {ElapsedMs:F2}ms.")]
        public static partial void LogMessageConsumed(ILogger logger, string messageName, long elapsedMs);
    }
}
