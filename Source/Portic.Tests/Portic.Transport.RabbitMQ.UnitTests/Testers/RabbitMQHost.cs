using Microsoft.Extensions.Hosting;
using Portic.Extensions;
using Portic.Tests.Common.Helpers;
using Portic.Transport.RabbitMQ.Extensions;
using Testcontainers.RabbitMq;

namespace Portic.Transport.RabbitMQ.UnitTests.Testers
{
    internal static class RabbitMQHost
    {
        public static IHost CreateHost(RabbitMqContainer container)
        {
            var host = Host.CreateApplicationBuilder();

            host.Services.AddPortic(configurator =>
            {
                configurator.ConfigureConsumer<TestMessage, TestMessageConsumer>();

                configurator.UsingRabbitMQ(rabbit =>
                {
                    rabbit.WithConnectionString(container.GetConnectionString());
                });
            });

            return host.Build();
        }
    }
}
