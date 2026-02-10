namespace Portic.Exceptions
{
    public sealed class MessageDefinitionNotFoundException : PorticException
    {
        public MessageDefinitionNotFoundException(string? messageName) : base($"Message definition was not found for message '{messageName}' and could not be processed")
        {

        }
    }
}