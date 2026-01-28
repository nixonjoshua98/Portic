using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portic.Extensions;
using Portic.Samples.Alpha;
using Portic.Transport.RabbitMQ.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog();

builder.Services.AddHostedService<PingPublisher>();

builder.Services.AddPortic(configurator =>
{
    configurator.ConfigureConsumer<PingMessage, PingConsumer>()
        .WithEndpointName("PingEndpoint");

    configurator.ConfigureEndpoint("PingEndpoint")
        .WithPrefetchCount(16);

    configurator.UsingRabbitMQ();
});

var host = builder.Build();

await host.RunAsync();