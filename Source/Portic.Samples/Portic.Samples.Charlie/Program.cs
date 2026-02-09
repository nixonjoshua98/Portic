using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portic.Extensions;
using Serilog;
using Portic.Transport.InMemory.Extensions;
using Portic.Samples.Charlie;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog();

builder.Services.AddHostedService<PingPublisher>();

builder.Services.AddPortic(configurator =>
{
    configurator.ConfigureConsumer<PingMessage, PingConsumer>();

    configurator.ConfigureConsumer<PingMessage, PingConsumerV2>();

    configurator.UsingInMemory();
});

var host = builder.Build();

await host.RunAsync();