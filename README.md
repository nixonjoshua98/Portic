# ⚓ Portic

**The lightweight, contract-first messaging framework for .NET.**

[![NuGet](https://img.shields.io/nuget/v/Portic.svg?style=flat-square)](https://www.nuget.org/packages/Portic)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/github/actions/workflow/status/nixonjoshua98/Portic/PublishNuget.yaml?style=flat-square)](https://github.com/nixonjoshua98/Portic/actions)

Portic is a lightweight, contract-first messaging framework for .NET prioritising strict message schema integrity.

## ⚠️ Versioning
Portic is currently in **early development**. The API is subject to change and may introduce breaking changes as part of minor version bumps.

## ✨ Features

- **Contract-Based Messaging** - Strongly-typed message contracts ensure compile-time safety and enforce strict schema integrity across your distributed system
- **Middleware Pipeline** - Extensible middleware support for cross-cutting concerns like retry policies, logging, validation, and more
- **Transport Agnostic** - Swap message transports with minimal application code changes
- **Lightweight** - Minimal dependencies and overhead, focused on doing one thing well

## 📦 Packages

| Package | Version | Description |
|---------|---------|-------------|
| [Portic](https://www.nuget.org/packages/Portic) | [![NuGet](https://img.shields.io/nuget/v/Portic.svg)](https://www.nuget.org/packages/Portic) | Core messaging framework and abstractions |
| [Portic.Transport.RabbitMQ](https://www.nuget.org/packages/Portic.Transport.RabbitMQ) | [![NuGet](https://img.shields.io/nuget/v/Portic.Transport.RabbitMQ.svg)](https://www.nuget.org/packages/Portic.Transport.RabbitMQ) | RabbitMQ transport implementation |
| [Portic.Middleware.Polly](https://www.nuget.org/packages/Portic.Middleware.Polly) | [![NuGet](https://img.shields.io/nuget/v/Portic.Middleware.Polly.svg)](https://www.nuget.org/packages/Portic.Middleware.Polly) | Polly middleware for resilience policies |


## 📚 Samples
For complete working examples, check out the [Samples](https://github.com/nixonjoshua98/Portic/tree/master/Source/Portic.Samples).

**Installation**

```bash
# Core package (optional - included with transport packages)
dotnet add package Portic

# Transport package
dotnet add package Portic.Transport.RabbitMQ
```