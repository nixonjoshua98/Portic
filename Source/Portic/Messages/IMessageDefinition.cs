namespace Portic.Messages
{
    public interface IMessageDefinition
    {
        Type MessageType { get; }
        string Name { get; }

        T GetPropertyOrDefault<T>(string key, T defaultValue);
    }
}
