using Polly;
using Portic.Abstractions;

namespace Portic.Middleware.Polly.Configuration
{
    public interface IPollyMiddlewareConfigurator
    {
        internal bool UseScopePerExecution { get; }
        internal ResiliencePipelineBuilder Builder { get; }

        IPollyMiddlewareConfigurator WithRetryCount(byte retryCount, TimeSpan? delay = null);
        IPollyMiddlewareConfigurator WithScopePerExecution(bool value = true);
    }
}
