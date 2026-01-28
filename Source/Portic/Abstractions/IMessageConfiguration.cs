namespace Portic.Abstractions
{
    public interface IMessageConfiguration
    {
        Type MessageType { get; }
        string Name { get; }

        T GetPropertyOrDefault<T>(string key, T defaultValue);
    }
}
