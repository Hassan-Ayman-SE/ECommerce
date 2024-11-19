using RabbitMQ.Client;

namespace OrderService.RabbitMQ.Connection
{
    public interface IRabbitMqConnection
    {
        IConnection Connection { get; }

    }
}
