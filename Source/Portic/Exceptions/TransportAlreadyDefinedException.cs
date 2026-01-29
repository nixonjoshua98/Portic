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

    public sealed class TransportNotDefinedException : PorticException
    {
        private TransportNotDefinedException() :
            base("No transport has been defined. Please define a transport using 'SetTransportDefinition' method in the Portic configurator.")
        {

        }

        internal static TransportNotDefinedException Create() => new();
    }
}