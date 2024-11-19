using RabbitMQ.Client;

namespace InventoryService.RabbitMQ.Connection
{
    public interface IRabbitMqConnection
    {
        IConnection Connection { get; }
    }
}
