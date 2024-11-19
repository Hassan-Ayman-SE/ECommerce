using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using InventoryService.DTOs;
using InventoryService.Repositories.Interfaces;
using InventoryService.RabbitMQ.Connection;

namespace InventoryService.RabbitMQ
{
    public class RabbitMqConsumer
    {
        private readonly IRabbitMqConnection _connection;
        private readonly IInventory _inventoryRepository;
        private readonly IMessageProducer _messageProducer;

        public RabbitMqConsumer(IRabbitMqConnection connection, IInventory inventoryRepository, IMessageProducer messageProducer)
        {
            _connection = connection;
            _inventoryRepository = inventoryRepository;
            _messageProducer = messageProducer;
        }

        public void StartConsuming()
        {
            using var channel = _connection.Connection.CreateModel();
            channel.QueueDeclare(queue: "OrderCreated", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var orderEvent = JsonSerializer.Deserialize<InventoryDto>(message);

                if (orderEvent != null)
                {
                    var inventoryDto = new InventoryDto
                    {
                        ProductId = orderEvent.ProductId,
                        Quantity = orderEvent.Quantity
                    };

                    var isInStock = await _inventoryRepository.CheckAndReduceStock(inventoryDto);

                    if (isInStock)
                    {
                        // Publish Inventory Updated event
                        var inventoryUpdatedEvent = new InventoryDto
                        {
                            ProductId = inventoryDto.ProductId,
                            Quantity = inventoryDto.Quantity
                        };
                        _messageProducer.SendMessage(inventoryUpdatedEvent);
                    }
                    else
                    {
                        // Publish OutOfStock event
                        var outOfStockEvent = new OutOfStockDto
                        {
                            ProductId = inventoryDto.ProductId
                        };
                        _messageProducer.SendMessage(outOfStockEvent);
                    }
                }
            };

            channel.BasicConsume(queue: "OrderCreated", autoAck: true, consumer: consumer);
        }
    }
}
