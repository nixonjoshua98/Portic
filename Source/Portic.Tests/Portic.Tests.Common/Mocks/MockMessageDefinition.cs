using Portic.Messages;

namespace Portic.Tests.Common.Mocks
{
    public sealed class MockMessageDefinition(Type messageType, string name) : IMessageDefinition
    {
        public Type MessageType { get; } = messageType;
        public string Name { get; } = name;

        public T GetPropertyOrDefault<T>(string key, T defaultValue)
        {
            throw new NotImplementedException();
        }
    }
}