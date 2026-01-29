using Portic.Transport;

namespace Portic.Exceptions
{
    public sealed class TransportAlreadyDefinedException : PorticException
    {
        private TransportAlreadyDefinedException(string currentTransportName, string attemptedTransportName) :
            base($"Transport '{currentTransportName}' has already been defined and cannot be redefined to '{attemptedTransportName}'"
        )
        {

        }

        internal static TransportAlreadyDefinedException FromTransport(ITransportDefinition current, ITransportDefinition attempted) =>
            new TransportAlreadyDefinedException(current.DisplayName, attempted.DisplayName);
    }
}