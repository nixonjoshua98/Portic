namespace Portic.Transport.RabbitMQ.Topology
{
    public interface IRabbitMQTransportConfigurator
    {
        IRabbitMQTransportConfigurator WithHost(string hostName);
        IRabbitMQTransportConfigurator WithPort(int port);
    }
}
