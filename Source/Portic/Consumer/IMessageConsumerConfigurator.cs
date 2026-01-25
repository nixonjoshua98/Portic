namespace Portic.Consumer
{
    public interface IMessageConsumerConfigurator
    {
        Type MessageType { get; }

        IMessageConsumerConfigurator WithEndpointName(string endpointName);
    }
}
