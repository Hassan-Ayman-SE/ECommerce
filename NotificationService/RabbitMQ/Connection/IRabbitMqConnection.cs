using RabbitMQ.Client;

namespace NotificationService.RabbitMQ.Connection
{
    public interface IRabbitMqConnection
    {
        IConnection Connection { get; }
    }
}
