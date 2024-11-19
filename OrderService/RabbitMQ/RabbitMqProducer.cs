using OrderService.RabbitMQ.Connection;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.RabbitMQ
{
    public class RabbitMqProducer : IMessageProducer
    {
        private readonly IRabbitMqConnection _connection;

        public RabbitMqProducer(IRabbitMqConnection connection)
        {
            _connection = connection;
        }

        public void SendMessage<T>(T message)
        {
            using var channel = _connection.Connection.CreateModel();
            channel.QueueDeclare("OrderCreated", exclusive: false);
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "", routingKey: "orders", body: body);
        }
    }
}
