using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portic.Extensions;
using Portic.Middleware.Polly.Extensions;
using Portic.Samples.Bravo;
using Portic.Transport.RabbitMQ.Extensions;
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
    configurator.ConfigureConsumer<PingMessage, PingConsumer>();

    configurator.ConfigureConsumer<PingMessage, PingConsumer2>()
        .WithEndpointName("ping-consumer-two");

    configurator.Use<LoggingMiddleware>();

    configurator.UsePolly(polly => polly
        .WithRetryCount(3, TimeSpan.FromMilliseconds(100))
        .WithScopePerExecution()
    );

    configurator.UsingRabbitMQ();
});

var host = builder.Build();

await host.RunAsync();