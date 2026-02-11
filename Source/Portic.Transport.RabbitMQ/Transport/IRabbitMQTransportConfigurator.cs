namespace Portic.Transport.RabbitMQ.Transport
{
    public interface IRabbitMQTransportConfigurator
    {
        IRabbitMQTransportConfigurator WithConnectionString(string connectionString);
        IRabbitMQTransportConfigurator WithHost(string hostName);
        IRabbitMQTransportConfigurator WithPassword(string password);
        IRabbitMQTransportConfigurator WithPort(int port);
        IRabbitMQTransportConfigurator WithUserName(string username);
    }
}
