using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portic.Extensions;
using Portic.Tests.Common.Helpers;
using Portic.Transport.RabbitMQ.Extensions;
using Portic.Transport.RabbitMQ.UnitTests.Consumers;
using Testcontainers.RabbitMq;

namespace Portic.Transport.RabbitMQ.UnitTests.Helpers
{
    internal static class RabbitMQHost
    {
        public static IHost CreateHost(RabbitMqContainer container)
        {
            var host = Host.CreateApplicationBuilder();

            host.Services.AddPortic(configurator =>
            {
                configurator.ConfigureConsumer<TestMessage, TrackableConsumer>();

                configurator.UsingRabbitMQ(rabbit =>
                {
                    rabbit.WithConnectionString(container.GetConnectionString());
                });
            });

            host.Services.AddSingleton(
                new TaskCompletionSource<TestMessage>()
            );

            return host.Build();
        }
    }
}
