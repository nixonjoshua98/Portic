namespace Portic.Exceptions
{
    public sealed class MessageTypeNotFoundException(string message) : PorticException(message)
    {
        public static MessageTypeNotFoundException FromName(string? name) =>
            new($"ConsumerConfiguration was not found for message '{name}' and could not be processed");
    }
}