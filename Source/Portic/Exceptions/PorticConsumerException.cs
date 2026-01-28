using Portic.Consumer;

namespace Portic.Exceptions
{
    public sealed class PorticConsumerException<TMessage> : PorticException
    {
        public IConsumerContext<TMessage> Context { get; }
        public bool ShouldRedeliver { get; }

        private PorticConsumerException(
            Exception innerException,
            IConsumerContext<TMessage> context,
            bool shouldRedeliver
        ) : base($"Message consumption failed for {context.MessageId}, redelivery requested (Delivery {context.DeliveryCount + 1} of {context.MaxRedeliveryAttempts})", innerException)
        {
            Context = context;
            ShouldRedeliver = shouldRedeliver;
        }

        internal static PorticConsumerException<TMessage> ForRedelivery(Exception innerException, IConsumerContext<TMessage> context) => new(innerException, context, shouldRedeliver: true);
    }
}