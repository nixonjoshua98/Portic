using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Portic.Abstractions;
using Portic.Exceptions;
using Portic.Logging;

namespace Portic.Consumer
{
    internal sealed class ConsumerExecutor(
        IConsumerContextFactory _contextFactory,
        ILogger<ConsumerExecutor> _logger,
        IPorticConfiguration _configuration
    ) : IConsumerExecutor
    {
        public async Task ExecuteAsync<TMessage>(ConsumerExecutorContext<TMessage> executorContext, CancellationToken cancellationToken)
        {
            var context = _contextFactory.CreateContext(
                executorContext,
                cancellationToken
            );

            var pipeline = BuildPipeline(context);

            await pipeline(context);
        }

        private ConsumerMiddlewareDelegate BuildPipeline<TMessage>(IConsumerContext<TMessage> context)
        {
            ConsumerMiddlewareDelegate pipeline = async _ =>
            {
                await ExecuteConsumerAsync(context);
            };

            foreach (var middlewareType in _configuration.Middleware.Reverse())
            {
                AddMiddlewareToPipeline(context, ref pipeline, middlewareType);
            }

            return pipeline;
        }

        private static void AddMiddlewareToPipeline<TMessage>(IConsumerContext<TMessage> context, ref ConsumerMiddlewareDelegate pipeline, Type middlewareType)
        {
            var currentPipeline = pipeline;

            pipeline = async (ctx) =>
            {
                var middleware = ActivatorUtilities.CreateInstance(context.Services, middlewareType) as IConsumerMiddleware
                    ?? throw new InvalidOperationException($"Failed to create middleware instance of type '{middlewareType.Name}'");

                await middleware.InvokeAsync(ctx, currentPipeline);
            };
        }

        private async Task ExecuteConsumerAsync<TMessage>(IConsumerContext<TMessage> context)
        {
            var consumerInst = ActivatorUtilities.CreateInstance(context.Services, context.Consumer.ConsumerType) as IConsumer<TMessage>
                ?? throw UnknownMessageException.FromName(context.Consumer.Message.Name);

            await consumerInst.ConsumeAsync(context);

            _logger.LogMessageConsumed(context.Consumer.Message.Name);
        }
    }
}
