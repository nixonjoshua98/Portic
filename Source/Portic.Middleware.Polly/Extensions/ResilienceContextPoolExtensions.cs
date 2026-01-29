using Microsoft.Extensions.Logging;
using Polly;
using Portic.Consumers;
using System.Diagnostics.CodeAnalysis;

namespace Portic.Middleware.Polly.Extensions
{
    internal static class ResilienceContextPoolExtensions
    {
        private static readonly ResiliencePropertyKey<IConsumerContext> ContextKey = new ResiliencePropertyKey<IConsumerContext>("Context");
        private static readonly ResiliencePropertyKey<ILogger> LoggerKey = new ResiliencePropertyKey<ILogger>("Logger");

        public static void SetLoggingProperties(this ResilienceContext context, IConsumerContext consumerContext, ILogger logger)
        {
            context.Properties.Set(ContextKey, consumerContext);
            context.Properties.Set(LoggerKey, logger);
        }

        public static bool TryGetLoggingProperties(this ResilienceContext context, [NotNullWhen(true)] out IConsumerContext? consumeContext, [NotNullWhen(true)] out ILogger? logger)
        {
            logger = null;

            return
                context.Properties.TryGetValue(ContextKey, out consumeContext) &&
                context.Properties.TryGetValue(LoggerKey, out logger);
        }
    }
}
