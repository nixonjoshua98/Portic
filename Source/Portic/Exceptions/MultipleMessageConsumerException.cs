namespace Portic.Exceptions
{
    public sealed class MultipleMessageConsumerException : PorticException
    {
        internal MultipleMessageConsumerException(string endpointName, Type messageType) :
            base($"Endpoint '{endpointName}' has been configured with multiple consumers for the same message type '{messageType.Name}'")
        {

        }
    }
}
