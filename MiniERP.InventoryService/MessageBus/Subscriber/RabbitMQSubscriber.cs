using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace MiniERP.InventoryService.MessageBus.Subscriber
{
    public class RabbitMQSubscriber : BackgroundService
    {
        private readonly ILogger<RabbitMQSubscriber> _logger;
        private readonly IMessageRouter _router;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly List<Tuple<string, string>> _queues = new();
        public RabbitMQSubscriber(IConfiguration configuration,
                                  ILogger<RabbitMQSubscriber> logger,
                                  IMessageRouter router)
        {
            _logger = logger;
            _router = router;

            ConnectionFactory factory = GetConfiguredFactory(configuration);

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _queues.Add(new("inventory", "movement.command"));
            _queues.Add(new("article", "inventory"));

            _logger.LogInformation("---> Connected to RabbitMQ Message Bus : {date}", DateTime.UtcNow);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();


            foreach(Tuple<string, string> queue in _queues)
            {
                DeclareBindAndConsume(queue.Item1, queue.Item2);
            }

            return Task.CompletedTask;
        }

        private void DeclareBindAndConsume(string exchange, string queue)
        {
            string queueName = DeclareAndBindQueue(_channel, exchange, queue);

            EventingBasicConsumer consumer = new RabbitMQConsumer(_logger, _router, _channel);

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        private string DeclareAndBindQueue(IModel model, string exchange, string queue)
        {
            model.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);

            string queueName = model.QueueDeclare().QueueName;

            model.QueueBind(queue: queueName,
                               exchange: exchange,
                               routingKey: queue);
            return queueName;

        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();

            base.Dispose();
        }

        private ConnectionFactory GetConfiguredFactory(IConfiguration configuration)
        {
            return new ConnectionFactory()
            {
                HostName = configuration["RabbitMQHost"],
                Port = int.Parse(configuration["RabbitMQPort"]!),
                UserName = configuration["RabbitMQUser"],
                Password = configuration["RabbitMQPassword"],
                VirtualHost = "/"

            };
        }
    }
}
