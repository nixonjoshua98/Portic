namespace Portic.Exceptions
{
    public sealed class DuplicateMessageNameException : PorticException
    {
        internal DuplicateMessageNameException() : base($"Duplicate message name detected. Message names must be unique")
        {

        }
    }
}
