namespace Portic.Consumer
{
    public interface IConsumerMiddleware
    {
        Task InvokeAsync(IConsumerContext context, ConsumerMiddlewareDelegate next);
    }

    public delegate Task ConsumerMiddlewareDelegate(IConsumerContext context);
}
