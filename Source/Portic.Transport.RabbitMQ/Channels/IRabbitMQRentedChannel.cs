using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Channels
{
    internal interface IRabbitMQRentedChannel : IDisposable
    {
        IChannel Channel { get; }
    }
}