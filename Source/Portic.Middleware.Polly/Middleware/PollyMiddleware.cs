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
        private readonly PollyMiddlewareOptions _options = _middlewareOptions.Value;

        public async Task InvokeAsync(IConsumerContext context, ConsumerMiddlewareDelegate next)
        {
            var resilienceContext = ResilienceContextPool.Shared.Get(context.CancellationToken);

            try
            {
                resilienceContext.SetLoggingProperties(context, _logger);

                await _options.Pipeline.ExecuteAsync(async _ => await ExecuteAsync(context, next), resilienceContext);
            }
            catch (Exception ex)
            {
                _logger.LogFailedResiliencePipeline(ex, context.MessageId);

                throw;
            }
            finally
            {
                ResilienceContextPool.Shared.Return(resilienceContext);
            }
        }

        private async Task ExecuteAsync(IConsumerContext context, ConsumerMiddlewareDelegate next)
        {
            var originalServiceProvider = context.Services;

            AsyncServiceScope? createdScope = null;

            if (_options.UseScopePerExecution)
            {
                createdScope = context.Services.CreateAsyncScope();

                context.WithServiceProvider(createdScope.Value.ServiceProvider);
            }

            try
            {
                await next(context);
            }
            finally
            {
                if (createdScope.HasValue)
                {
                    await createdScope.Value.DisposeAsync();

                    context.WithServiceProvider(originalServiceProvider);
                }
            }
        }
    }
}