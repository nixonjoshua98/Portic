using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portic.Extensions;
using Portic.Samples.Alpha;
using Portic.Transport.RabbitMQ.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging
    .SetMinimumLevel(LogLevel.Debug);

builder.Services.AddHostedService<PingPublisher>();

builder.Services.AddPortic(configurator =>
{
    configurator.ConfigureEndpoint("PingEndpoint")
        .WithPrefetchCount(64)
        .WithChannelCount(2);

    configurator.ConfigureConsumer<PingMessage, PingConsumer>()
        .WithEndpointName("PingEndpoint");

    configurator.UsingRabbitMQ(configurator =>
    {

    });
});

var host = builder.Build();

await host.RunAsync();