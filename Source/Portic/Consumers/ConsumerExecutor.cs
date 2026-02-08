using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Portic.Configuration;
using Portic.Exceptions;
using Portic.Logging;
using Portic.Transport;

namespace Portic.Consumers
{
    internal sealed class ConsumerExecutor(
        ILogger<ConsumerExecutor> _logger,
        IPorticConfiguration _configuration,
        IServiceScopeFactory _scopeFactory
    ) : IConsumerExecutor
    {
        public async Task ExecuteAsync<TMessage>(TransportMessageReceived<TMessage> message, CancellationToken cancellationToken)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            var context = new ConsumerContext<TMessage>(
                message.MessageId,
                message.Message,
                message.DeliveryCount,
                scope.ServiceProvider,
                message.ConsumerDefinition,
                message.EndpointDefinition,
                message.Settlement,
                cancellationToken
            );

            var pipeline = BuildPipeline(context);

            try
            {
                await pipeline(context);

                await context.Settlement.CompleteAsync(cancellationToken);

                _logger.LogMessageConsumed(context.MessageId);
            }
            catch (Exception exception) when (context.DeliveryCount < context.MaxRedeliveryAttempts)
            {
                await context.Settlement.DeferAsync(exception, cancellationToken);

                _logger.LogDeferredMessage(context.MessageId, exception, context.DeliveryCount, context.MaxRedeliveryAttempts);
            }
            catch (Exception exception)
            {
                await context.Settlement.FaultAsync(exception, cancellationToken);

                _logger.LogFaultedMessage(context.MessageId, exception);
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
                    ?? throw MiddlewareCreationException.FromType(middlewareType);

                await middleware.InvokeAsync(context, currentPipeline);
            };
        }

        private static async Task ExecuteConsumerAsync<TMessage>(IConsumerContext<TMessage> context)
        {
            var consumerInst = ActivatorUtilities.GetServiceOrCreateInstance(context.Services, context.ConsumerDefinition.ConsumerType) as IConsumer<TMessage>
                ?? throw MessageTypeNotFoundException.FromName(context.ConsumerDefinition.Message.Name);

            await consumerInst.ConsumeAsync(context);
        }
    }
}