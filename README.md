# ⚓ Portic

**The lightweight, contract-first messaging framework for .NET.**

[![NuGet](https://img.shields.io/nuget/v/Portic.svg?style=flat-square)](https://www.nuget.org/packages/Portic)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/github/actions/workflow/status/nixonjoshua98/Portic/PublishNuget.yaml?style=flat-square)](https://github.com/nixonjoshua98/Portic/actions)

Portic is a lightweight, contract-first messaging framework for .NET. Built, it prioritizes strict message schema integrity and low-latency routing. By stripping away infrastructure bloat, Portic provides a high-performance, decoupled foundation for modern microservices.

## 📦 Installation

Install the core library + RabbitMQ transport via NuGet:

```bash
dotnet add package Portic
dotnet add package Portic.Transport.RabbitMQ
```