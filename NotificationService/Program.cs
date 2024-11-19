using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.RabbitMQ;
using NotificationService.RabbitMQ.Connection;

namespace NotificationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var consumer = host.Services.GetRequiredService<RabbitMqConsumer>();
            consumer.StartConsuming();

            Console.WriteLine("Notification Service is running. press enter to exit");
            Console.ReadLine(); 
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
                    services.AddSingleton<RabbitMqConsumer>();

                    services.AddLogging(builder => builder.AddConsole());
                });
    }
}
