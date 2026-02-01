namespace Portic.Exceptions
{
    public sealed class DuplicateMessageNameException : PorticException
    {
        public string MessageName { get; }

        private DuplicateMessageNameException(string messageName) :
            base($"Duplicate message name detected: '{messageName}'. Message names must be unique")
        {
            MessageName = messageName;
        }

        public static DuplicateMessageNameException FromName(string messageName) => new(messageName);
    }
}
