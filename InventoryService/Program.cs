using InventoryService.Data;
using InventoryService.RabbitMQ;
using InventoryService.RabbitMQ.Connection;
using InventoryService.Repositories.Interfaces;
using InventoryService.Repositories.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace InventoryService
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
            builder.Services.AddDbContext<InventoryDbContext>(opt => opt.UseSqlServer(connectionString));

            builder.Services.AddScoped<IInventory, InventoryRepository>();
            builder.Services.AddSingleton<IRabbitMqConnection>(new RabbitMqConnection());
            builder.Services.AddSingleton<IMessageProducer, RabbitMqProducer>();
            builder.Services.AddSingleton<RabbitMqConsumer>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            // Start RabbitMQ Consumer
            var rabbitMqConsumer = app.Services.GetRequiredService<RabbitMqConsumer>();
            Task.Run(() => rabbitMqConsumer.StartConsuming());
            app.Run();
        }
    }
}
