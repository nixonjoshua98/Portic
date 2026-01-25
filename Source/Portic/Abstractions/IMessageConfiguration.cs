namespace Portic.Abstractions
{
    public interface IMessageConfiguration
    {
        Type MessageType { get; }

        string GetName();
    }
}
