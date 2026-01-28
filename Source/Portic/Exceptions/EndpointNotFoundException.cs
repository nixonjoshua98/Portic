namespace Portic.Exceptions
{
    public sealed class EndpointNotFoundException : PorticException
    {
        public string EndpointName { get; }

        private EndpointNotFoundException(string endpointName) : base($"The endpoint '{endpointName}' was not found.")
        {
            EndpointName = endpointName;
        }

        public static EndpointNotFoundException ForEndpoint(string endpointName)
        {
            return new EndpointNotFoundException(endpointName);
        }
    }
}