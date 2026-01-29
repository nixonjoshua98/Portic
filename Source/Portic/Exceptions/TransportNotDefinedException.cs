namespace Portic.Exceptions
{
    public sealed class TransportNotDefinedException : PorticException
    {
        private TransportNotDefinedException() : base("No transport has been defined for Portic.")
        {

        }

        internal static TransportNotDefinedException CreateNew() => new();
    }
}