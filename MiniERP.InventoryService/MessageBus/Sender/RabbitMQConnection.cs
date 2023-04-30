using MiniERP.InventoryService.MessageBus.Sender.Contracts;
using RabbitMQ.Client;

namespace MiniERP.InventoryService.MessageBus.Sender
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private const string CreatedLogFormat = "---------> RabbitMQ : Client Connection Created";
        private const string RecoveryLogFormat = "---------> RabbitMQ : Client Connection Recovery Success";
        private const string ShutdownLogFormat = "---------> RabbitMQ : Client Connection Shutting Down";

        private readonly ILogger<RabbitMQConnection> _logger;
        public IConnection Connection { get; }
        public RabbitMQConnection(IConfiguration configuration,
                                  ILogger<RabbitMQConnection> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQHost"],
                Port = int.Parse(configuration["RabbitMQPort"]!),
                UserName = configuration["RabbitMQUser"],
                Password = configuration["RabbitMQPassword"],
                VirtualHost = "/",
                AutomaticRecoveryEnabled = true
            };

            var connection = factory.CreateConnection() as IAutorecoveringConnection ?? throw new ArgumentException(nameof(IAutorecoveringConnection));

            connection.ConnectionShutdown += Connection_ConnectionShutdown;
            connection.RecoverySucceeded += Connection_RecoverySucceeded;

            Connection = connection;

            _logger.LogInformation(CreatedLogFormat);
        }

        private void Connection_RecoverySucceeded(object? sender, EventArgs e)
        {
            _logger.LogInformation(RecoveryLogFormat);
        }

        private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
                _logger.LogInformation(ShutdownLogFormat);
        }
    }
}
