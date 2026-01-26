using Polly;

namespace Portic.Middleware.Polly.Configuration
{
    public interface IPollyMiddlewareConfigurator
    {
        ResiliencePipelineBuilder Builder { get; }
    }

    internal sealed class PollyMiddlewareConfigurator : IPollyMiddlewareConfigurator
    {
        public ResiliencePipelineBuilder Builder { get; } = new ResiliencePipelineBuilder();
    }
}
