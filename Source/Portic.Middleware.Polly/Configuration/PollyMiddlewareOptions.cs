using Polly;

namespace Portic.Middleware.Polly.Configuration
{
    internal sealed class PollyMiddlewareOptions
    {
        public ResiliencePipeline Pipeline { get; internal set; } = ResiliencePipeline.Empty;
        public bool UseScopePerExecution { get; internal set; }
    }
}
