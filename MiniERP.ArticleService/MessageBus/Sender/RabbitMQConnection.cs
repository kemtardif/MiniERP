using MiniERP.ArticleService.MessageBus.Sender.Contracts;
using RabbitMQ.Client;

namespace MiniERP.ArticleService.MessageBus.Sender
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMQConnection> _logger;

        public IConnection Connection => _connection;

        public RabbitMQConnection(IConfiguration configuration,
            ILogger<RabbitMQConnection> logger)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQHost"],
                Port = int.Parse(configuration["RabbitMQPort"]!),
                UserName = configuration["RabbitMQUser"],
                Password = configuration["RabbitMQPassword"],
                VirtualHost = "/",
                AutomaticRecoveryEnabled = true

            };
            var conn = factory.CreateConnection() as IAutorecoveringConnection ?? throw new ArgumentException(nameof(_connection));
            conn.ConnectionShutdown += _connection_ConnectionShutdown;
            conn.RecoverySucceeded += _connection_RecoverySucceeded;

            _connection = conn;
            _logger = logger;
            _logger.LogInformation("---------> RabbitMQ : Client Connection Created");
        }

        private void _connection_RecoverySucceeded(object? sender, EventArgs e)
        {
            _logger.LogInformation("---------> RabbitMQ : Client Connection Recovery Success");
        }

        private void _connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("---------> RabbitMQ : Client Connection Shutting Down");
        }
    }
}
