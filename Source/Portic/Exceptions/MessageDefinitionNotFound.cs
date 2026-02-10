namespace Portic.Exceptions
{
    public sealed class MessageDefinitionNotFound : PorticException
    {
        public MessageDefinitionNotFound(string? messageName) : base($"Message definition was not found for message '{messageName}' and could not be processed")
        {

        }
    }
}