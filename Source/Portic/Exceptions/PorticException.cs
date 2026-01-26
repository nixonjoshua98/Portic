namespace Portic.Exceptions
{
    public abstract class PorticException : Exception
    {
        protected PorticException(string message) : base(message)
        {

        }

        protected PorticException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }

    public sealed class PorticConsumerException(
        string message,
        Exception innerException,
        bool shouldRedeliver
    ) : PorticException(message, innerException)
    {
        public bool ShouldRedeliver { get; } = shouldRedeliver;

        public static PorticConsumerException ForRedelivery(string messageId, byte redeliveryCount, byte maxRedeliveryCount, Exception innerException) =>
            new($"Message consumption failed for {messageId} (Delivery {redeliveryCount} of {maxRedeliveryCount})", innerException, shouldRedeliver: true);
    }
}