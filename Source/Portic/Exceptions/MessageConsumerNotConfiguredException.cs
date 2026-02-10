namespace Portic.Exceptions
{
    public sealed class MessageConsumerNotConfiguredException : PorticException
    {
        public MessageConsumerNotConfiguredException(string? messageName, string endpointName) :
            base($"Endpoint '{endpointName}' is missing a configured consumer for message '{messageName}'")
        {

        }
    }
}