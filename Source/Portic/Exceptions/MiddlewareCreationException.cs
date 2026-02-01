namespace Portic.Exceptions
{
    public sealed class MiddlewareCreationException : PorticException
    {
        public Type MiddlewareType { get; }

        private MiddlewareCreationException(Type middlewareType, Exception? innerException = null) :
            base($"Failed to create middleware instance of type '{middlewareType.Name}'", innerException)
        {
            MiddlewareType = middlewareType;
        }

        public static MiddlewareCreationException FromType(Type middlewareType, Exception? innerException = null)
            => new(middlewareType, innerException);
    }
}
