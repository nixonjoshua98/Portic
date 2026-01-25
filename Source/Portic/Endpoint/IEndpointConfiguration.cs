using Portic.Consumer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Portic.Endpoint
{
    public interface IEndpointConfiguration
    {
        string Name { get; }
        IReadOnlyDictionary<string, IMessageConsumerConfiguration> Consumers { get; }

        bool TryGetConsumerForMessage(string messageName, [NotNullWhen(true)] out IMessageConsumerConfiguration? consumer);
    }
}
