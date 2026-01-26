using Microsoft.Extensions.Logging;
using Portic.Consumer;

namespace Portic.Samples.Bravo
{
    internal sealed class LoggingMiddleware(ILogger<LoggingMiddleware> _logger) : IConsumerMiddleware
    {
        public async Task InvokeAsync(IConsumerContext context, ConsumerMiddlewareDelegate next)
        {
            var messageName = context.MessageName;

            try
            {
                _logger.LogInformation("Processing message of type {MessageType}", messageName);

                await next(context);

                _logger.LogInformation("Successfully processed message of type {MessageType}", messageName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message of type {MessageType}", messageName);

                throw;
            }
        }
    }
}