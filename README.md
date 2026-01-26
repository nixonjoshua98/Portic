# ⚓ Portic

**The lightweight, contract-first messaging framework for .NET.**

[![NuGet](https://img.shields.io/nuget/v/Portic.svg?style=flat-square)](https://www.nuget.org/packages/Portic)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/github/actions/workflow/status/nixonjoshua98/Portic/PublishNuget.yaml?style=flat-square)](https://github.com/nixonjoshua98/Portic/actions)

Portic is a lightweight, contract-first messaging framework for .NET prioritising strict message schema integrity.

## ⚠️ Versioning
Portic is currently in **early development**. The API is subject to change and may introduce breaking changes as part of minor version bumps.

## 📚 Quick Start + Samples
For complete working examples, check out the [Samples](/Source/Portic.Samples).

```cs
builder.Services.AddPortic(configurator =>
{
    configurator.ConfigureConsumer<PingMessage, PingConsumer>();

    configurator.UsingRabbitMQ();
});

...

public class MyDependency(IMessageTransport _transport)
{
    public async Task PublishPing() 
    {
        await _transport.PublishAsync(
            new PingMessage()
        );
    }
}
```

## 📦 Installation
*Installing the core package alongside a transport package is not mandatory, as it will be included as part of the transport*

Install via NuGet

```bash
dotnet add package Portic
dotnet add package Portic.Transport.RabbitMQ
```