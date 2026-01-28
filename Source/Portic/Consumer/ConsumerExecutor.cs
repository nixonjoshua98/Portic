using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Portic.Abstractions;
using Portic.Exceptions;
using Portic.Logging;

namespace Portic.Consumer
{
    internal sealed class ConsumerExecutor(
        ILogger<ConsumerExecutor> _logger,
        IPorticConfiguration _configuration
    ) : IConsumerExecutor
    {
        public async Task ExecuteAsync<TMessage>(IConsumerContext<TMessage> context, CancellationToken cancellationToken)
        {
            var pipeline = BuildPipeline(context);

            try
            {
                await pipeline(context);
            }

            // Redelivery
            catch (Exception ex) when (context.DeliveryCount < context.MaxRedeliveryAttempts)
            {
                throw PorticConsumerException.ForRedelivery(
                    context.MessageId,
                    Convert.ToByte(context.DeliveryCount + 1),
                    context.MaxRedeliveryAttempts,
                    ex
                );
            }
        }

        private ConsumerMiddlewareDelegate BuildPipeline<TMessage>(IConsumerContext<TMessage> context)
        {
            ConsumerMiddlewareDelegate pipeline = async _ =>
            {
                await ExecuteConsumerAsync(context);
            };

            foreach (var middlewareType in _configuration.Middleware.Reverse())
            {
                AddMiddlewareToPipeline(ref pipeline, middlewareType);
            }

            return pipeline;
        }

        private static void AddMiddlewareToPipeline(ref ConsumerMiddlewareDelegate pipeline, Type middlewareType)
        {
            var currentPipeline = pipeline;

            pipeline = async context =>
            {
                var middleware = ActivatorUtilities.GetServiceOrCreateInstance(context.Services, middlewareType) as IConsumerMiddleware
                    ?? throw new InvalidOperationException($"Failed to create middleware instance of type '{middlewareType.Name}'");

                await middleware.InvokeAsync(context, currentPipeline);
            };
        }

        private async Task ExecuteConsumerAsync<TMessage>(IConsumerContext<TMessage> context)
        {
            var consumerInst = ActivatorUtilities.GetServiceOrCreateInstance(context.Services, context.ConsumerConfiguration.ConsumerType) as IConsumer<TMessage>
                ?? throw MessageTypeNotFoundException.FromName(context.ConsumerConfiguration.Message.Name);

            await consumerInst.ConsumeAsync(context);

            _logger.LogMessageConsumed(context.ConsumerConfiguration.Message.Name);
        }
    }
}