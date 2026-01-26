using Polly;
using System;
using System.Collections.Generic;
using System.Text;

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
