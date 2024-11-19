using RabbitMQ.Client;

namespace InventoryService.RabbitMQ.Connection
{
    public class RabbitMqConnection : IRabbitMqConnection, IDisposable
    {
        private IConnection? _connection;
        public IConnection Connection => _connection ?? throw new InvalidOperationException("Connection has not been initialized.");

        public RabbitMqConnection()
        {
            InitConnection();
        }

        private void InitConnection()
        {

            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
            };

            _connection = factory.CreateConnection();

        }
        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
