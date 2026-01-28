namespace Portic.Abstractions
{
    public interface IMessageConfigurator
    {
        internal Type MessageType { get; }

        IMessageConfigurator SetProperty(string key, object value);
        IMessageConfigurator WithName(string name);
    }
}