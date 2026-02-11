namespace Portic.Middleware
{
    internal sealed record MiddlewareDefinition(Type Type) : IMiddlewareDefinition;
}
