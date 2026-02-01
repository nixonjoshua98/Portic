namespace Portic.Exceptions
{
    public abstract class PorticException : Exception
    {
        protected PorticException(string message) : base(message)
        {

        }

        protected PorticException(string message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}