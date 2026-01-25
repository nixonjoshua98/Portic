namespace Portic.Exceptions
{
    public sealed class ConsumerNotFoundException(string message) : Exception(message)
    {
        public static ConsumerNotFoundException FromName(string? name) =>
            new($"Consumer not found for message '{name}'");
    }
}
