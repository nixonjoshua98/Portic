namespace Portic.Exceptions
{
    public sealed class TransportNotDefinedException : PorticException
    {
        internal TransportNotDefinedException() : base("No transport has been defined for Portic.")
        {

        }
    }
}