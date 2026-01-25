using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portic.Extensions;
using Portic.Samples.Alpha;
using Portic.Transport.RabbitMQ.Extensions;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog(
    new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .CreateLogger()
);

builder.Services.AddHostedService<PingPublisher>();

builder.Services.AddPortic(configurator =>
{
    configurator.ConfigureConsumer<PingMessage, PingConsumer>()
        .WithEndpointName("PingEndpoint");

    configurator.ConfigureEndpoint("PingEndpoint")
        .WithPrefetchCount(32);

    configurator.UsingRabbitMQ(configurator =>
    {
        configurator
            .WithHost("localhost")
            .WithPort(5672);
    });
});

var host = builder.Build();

await host.RunAsync();