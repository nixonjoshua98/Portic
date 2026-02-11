using System;
using System.Collections.Generic;
using System.Text;

namespace Portic.Middleware
{
    internal sealed record MiddlewareDefinition(Type Type) : IMiddlewareDefinition;
}
