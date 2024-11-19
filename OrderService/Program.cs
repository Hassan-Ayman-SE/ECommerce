using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.RabbitMQ;
using OrderService.RabbitMQ.Connection;
using OrderService.Repositories.Interfaces;
using OrderService.Repositories.Services;
using System.Text.Json.Serialization;

namespace OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure JSON options to handle object cycles
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            // Get the connection string settings 
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Configure database context
            builder.Services.AddDbContext<OrderDbContext>(opt => opt.UseSqlServer(connectionString));

            builder.Services.AddScoped<IOrder, OrderRepository>();
            builder.Services.AddSingleton<IRabbitMqConnection>(new RabbitMqConnection());
            builder.Services.AddScoped<IMessageProducer, RabbitMqProducer>();

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
