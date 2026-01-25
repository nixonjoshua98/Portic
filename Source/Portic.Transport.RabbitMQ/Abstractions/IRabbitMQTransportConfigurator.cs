namespace Portic.Transport.RabbitMQ.Abstractions
{
    public interface IRabbitMQTransportConfigurator
    {
        IRabbitMQTransportConfigurator WithHost(string hostName);
        IRabbitMQTransportConfigurator WithPort(int port);
    }
}
