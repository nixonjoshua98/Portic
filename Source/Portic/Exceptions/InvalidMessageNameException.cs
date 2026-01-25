namespace Portic.Exceptions
{
    public sealed class InvalidMessageNameException(string message) : Exception(message)
    {
        public static InvalidMessageNameException FromName(string? name) => new InvalidMessageNameException($"Invalid message '{name}'");
    }
}
