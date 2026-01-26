using Microsoft.Extensions.DependencyInjection;
using Portic.Exceptions;
using Portic.Middleware;

namespace Portic.Consumer
{
    internal sealed class TextMiddleware : IConsumerMiddleware
    {
        public async Task InvokeAsync(IConsumerContext context, ConsumerMiddlewareDelegate next)
        {
            await next(context);
        }
    }

    internal sealed class ConsumerExecutor(IConsumerContextFactory _contextFactory) : IConsumerExecutor
    {
        public async Task ExecuteAsync<TMessage>(ConsumerExecutorContext<TMessage> context, CancellationToken cancellationToken)
        {
            await ConsumeAsync(context, cancellationToken);
        }

        private async Task ConsumeAsync<TMessage>(ConsumerExecutorContext<TMessage> context, CancellationToken cancellationToken)
        {
            var consumerInst = ActivatorUtilities.CreateInstance(context.Services, context.Consumer.ConsumerType) as IConsumer<TMessage>
                ?? throw UnknownMessageException.FromName(context.Consumer.Message.Name);

            var consumerContext = _contextFactory.CreateContext(
                context.Payload,
                cancellationToken
            );

            await consumerInst.ConsumeAsync(consumerContext);
        }
    }
}
