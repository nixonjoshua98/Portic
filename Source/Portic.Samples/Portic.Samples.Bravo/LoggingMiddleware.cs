using Microsoft.Extensions.Logging;
using Portic.Consumers;

namespace Portic.Samples.Bravo
{
    internal sealed class LoggingMiddleware(ILogger<LoggingMiddleware> _logger) : IConsumerMiddleware
    {
        public async Task InvokeAsync(IConsumerContext context, ConsumerMiddlewareDelegate next)
        {
            try
            {
                await next(context);

                _logger.LogInformation("Successfully processed message '{PorticMessageId}'", context.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message '{PorticMessageId}'", context.MessageId);

                throw;
            }
        }
    }
}