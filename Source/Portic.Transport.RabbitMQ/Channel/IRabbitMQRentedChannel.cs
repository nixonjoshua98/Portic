using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Channel
{
    internal interface IRabbitMQRentedChannel : IDisposable
    {
        IChannel Channel { get; }
    }
}