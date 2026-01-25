using Portic.Consumer;
using Portic.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Portic.Endpoint
{
    public interface IEndpointConfiguration
    {
        string Name { get; }
        IReadOnlyDictionary<string, IConsumerConfiguration> Consumers { get; }

        T GetPropertyOrDefault<T>(string key, T defaultValue);
        bool TryGetConsumerForMessage(string messageName, [NotNullWhen(true)] out IConsumerConfiguration? consumer);
    }
}
