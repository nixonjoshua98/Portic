using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Portic.Consumer;
using Portic.Middleware.Polly.Configuration;
using Portic.Middleware.Polly.Logging;

namespace Portic.Middleware.Polly.Middleware
{
    internal sealed class PollyMiddleware(
        IOptions<PollyMiddlewareOptions> _middlewareOptions,
        ILogger<PollyMiddleware> _logger
    ) : IConsumerMiddleware
    {
        private readonly ResiliencePipeline _pipeline = _middlewareOptions.Value.Pipeline;

        public async Task InvokeAsync(IConsumerContext context, ConsumerMiddlewareDelegate next)
        {
            try
            {
                await _pipeline.ExecuteAsync(async _ => await ExecuteAsync(context, next), context.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogFailedResiliencePipeline(ex, context.MessageName);

                throw;
            }
        }

        private static async Task ExecuteAsync(IConsumerContext context, ConsumerMiddlewareDelegate next)
        {
            var originalServiceProvider = context.Services;

            await using var scope = context.Services.CreateAsyncScope();

            context.WithServiceProvider(scope.ServiceProvider);

            try
            {
                await next(context);
            }
            finally
            {
                context.WithServiceProvider(originalServiceProvider);
            }
        }
    }
}