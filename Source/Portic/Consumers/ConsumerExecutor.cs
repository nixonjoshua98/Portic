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
        IServiceScopeFactory _scopeFactory,
        IConsumerContextFactory _contextFactory
    ) : IConsumerExecutor
    {
        public async Task ExecuteAsync<TMessage>(TransportMessageReceived<TMessage> message, CancellationToken cancellationToken)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            var context = await _contextFactory.CreateAsync(
                message,
                scope.ServiceProvider,
                cancellationToken
            );

            var pipeline = BuildPipeline(context);

            try
            {
                await pipeline(context);

                await context.Settlement.CompleteAsync(cancellationToken);
            }
            catch (Exception exception) when (context.DeliveryCount < context.MaxRedeliveryAttempts)
            {
                await context.Settlement.DeferAsync(exception, cancellationToken);

                _logger.LogDeferedMessage(context.MessageId, exception, context.DeliveryCount, context.MaxRedeliveryAttempts);
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

        private async Task ExecuteConsumerAsync<TMessage>(IConsumerContext<TMessage> context)
        {
            var consumerInst = ActivatorUtilities.GetServiceOrCreateInstance(context.Services, context.ConsumerDefinition.ConsumerType) as IConsumer<TMessage>
                ?? throw MessageTypeNotFoundException.FromName(context.ConsumerDefinition.Message.Name);

            await consumerInst.ConsumeAsync(context);

            _logger.LogMessageConsumed(context.ConsumerDefinition.Message.Name);
        }
    }
}