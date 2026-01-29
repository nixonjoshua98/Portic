namespace Portic.Consumers
{
    public interface IConsumerConfigurator
    {
        Type MessageType { get; }

        IConsumerConfigurator WithEndpointName(string endpointName);
    }
}