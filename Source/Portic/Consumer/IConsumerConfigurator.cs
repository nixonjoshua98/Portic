namespace Portic.Consumer
{
    public interface IConsumerConfigurator
    {
        Type MessageType { get; }

        IConsumerConfigurator WithEndpointName(string endpointName);
    }
}