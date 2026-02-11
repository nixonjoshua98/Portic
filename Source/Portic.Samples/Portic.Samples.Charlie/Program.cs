using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portic.Extensions;
using Portic.Samples.Charlie;
using Portic.Transport.InMemory.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog();

builder.Services.AddHostedService<PingPublisher>();

builder.Services.AddPortic(configurator =>
{
    configurator.ConfigureConsumer<PingMessage, PingConsumer>()
        .WithEndpointName("test-endpoint");

    configurator.ConfigureConsumer<PingMessage, PingConsumer2>();

    configurator.UsingInMemory();
});

var host = builder.Build();

await host.RunAsync();