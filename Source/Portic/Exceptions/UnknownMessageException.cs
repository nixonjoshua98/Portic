namespace Portic.Exceptions
{
    public sealed class UnknownMessageException(string message) : Exception(message)
    {
        public static UnknownMessageException FromName(string? name) =>
            new($"Consumer was not found for message '{name}' and could not be processed");
    }
}
