namespace Portic.Exceptions
{
    public sealed class PorticConsumerException(
        string message,
        Exception innerException,
        bool shouldRedeliver
    ) : PorticException(message, innerException)
    {
        public bool ShouldRedeliver { get; } = shouldRedeliver;

        public static PorticConsumerException ForRedelivery(string messageId, byte redeliveryCount, byte maxRedeliveryCount, Exception innerException) =>
            new($"Message consumption failed for {messageId}, redelivery requested (Delivery {redeliveryCount} of {maxRedeliveryCount})", innerException, shouldRedeliver: true);
    }
}