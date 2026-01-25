namespace Portic.Exceptions
{
    public sealed class MessageConsumerNotFoundException(string message) : Exception(message)
    {
        public static MessageConsumerNotFoundException FromName(string? name) =>
            new MessageConsumerNotFoundException($"Consumer not found for message '{name}'");
    }
}
