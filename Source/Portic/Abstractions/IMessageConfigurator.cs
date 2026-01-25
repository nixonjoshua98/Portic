namespace Portic.Abstractions
{
    public interface IMessageConfigurator
    {
        internal Type MessageType { get; }

        IMessageConfigurator WithName(string name);
    }
}