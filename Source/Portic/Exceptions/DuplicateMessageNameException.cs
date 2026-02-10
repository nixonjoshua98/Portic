namespace Portic.Exceptions
{
    public sealed class DuplicateMessageNameException : PorticException
    {
        internal DuplicateMessageNameException(string messageName) :
            base($"Message name '{messageName}' is used multiple times. Message names must be unique")
        {

        }
    }
}
