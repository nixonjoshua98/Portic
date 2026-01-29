using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Portic.Consumers;
using Portic.Middleware.Polly.Configuration;
using Portic.Middleware.Polly.Extensions;

namespace Portic.Middleware.Polly.Middleware
{
    internal sealed class PollyMiddleware(IOptions<PollyMiddlewareOptions> _middlewareOptions, ILogger<PollyMiddleware> _logger) : IConsumerMiddleware
    {
        private readonly ResiliencePipeline _pipeline = _middlewareOptions.Value.Pipeline;

        public async Task InvokeAsync(IConsumerContext context, ConsumerMiddlewareDelegate next)
        {
            var resilienceContext = ResilienceContextPool.Shared.Get(context.CancellationToken);

            try
            {
                resilienceContext.SetLoggingProperties(context, _logger);

                await _pipeline.ExecuteAsync(async _ => await ExecuteAsync(context, next), resilienceContext);
            }
            catch (Exception ex)
            {
                _logger.LogFailedResiliencePipeline(ex, context.MessageName);

                throw;
            }
            finally
            {
                ResilienceContextPool.Shared.Return(resilienceContext);
            }
        }

        private static async Task ExecuteAsync(IConsumerContext context, ConsumerMiddlewareDelegate next)
        {
            /*
             * When executing a consumer in a retry policy, each consumer execution should run in
             * its own scope. This ensures any state changes from the previous attempt do not leak,
             * such as DbContext tracking changes. 
             * We could maybe consider making this configurable in the future.
             */

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