using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using NotificationService.DTOs;
using NotificationService.RabbitMQ.Connection;

namespace NotificationService.RabbitMQ
{
    public class RabbitMqConsumer
    {
        private readonly IRabbitMqConnection _connection;

        public RabbitMqConsumer(IRabbitMqConnection connection)
        {
            _connection = connection;
        }

        public void StartConsuming()
        {
            using var channel = _connection.Connection.CreateModel();

            // Declare queues to listen to both InventoryUpdated and OutOfStock events
            channel.QueueDeclare(queue: "InventoryUpdated", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "OutOfStock", durable: true, exclusive: false, autoDelete: false, arguments: null);

            // Event consumer for InventoryUpdatedEvent
            var inventoryUpdatedConsumer = new EventingBasicConsumer(channel);
            inventoryUpdatedConsumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var inventoryUpdatedEvent = JsonSerializer.Deserialize<InventoryDto>(message);

                if (inventoryUpdatedEvent != null)
                {
                    // Mock notification when stock is updated
                    Console.WriteLine($"Notification: Order for product {inventoryUpdatedEvent.ProductId} processed. Stock reduced by {inventoryUpdatedEvent.Quantity}.");
                }
            };

            // Event consumer for OutOfStockEvent
            var outOfStockConsumer = new EventingBasicConsumer(channel);
            outOfStockConsumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var outOfStockEvent = JsonSerializer.Deserialize<OutOfStockDto>(message);

                if (outOfStockEvent != null)
                {
                    // Mock notification when product is out of stock
                    Console.WriteLine($"Notification: Order for product {outOfStockEvent.ProductId} is waiting due to low stock.");
                }
            };

            // Start consuming both queues
            channel.BasicConsume(queue: "InventoryUpdated", autoAck: true, consumer: inventoryUpdatedConsumer);
            channel.BasicConsume(queue: "OutOfStock", autoAck: true, consumer: outOfStockConsumer);
        }
    }
}
