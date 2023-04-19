using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using MiniERP.InventoryService.MessageBus.Consumers;

namespace MiniERP.InventoryService.MessageBus.Subscriber
{
    public class RabbitMQSubscriber : BackgroundService
    {
        private readonly ILogger<RabbitMQSubscriber> _logger;
        private readonly IConsumerFactory _consumerFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private readonly List<Tuple<string, string>> _queues = new();

        private const string ArticleEXCH = "article";

        private const string ArticleCreateRK = "article.create";
        private const string ArticleDeleteRK = "article.delete";
        private const string ArticleUpdateRK = "article.update";
        public RabbitMQSubscriber(IConfiguration configuration,
                                  ILogger<RabbitMQSubscriber> logger,
                                  IConsumerFactory consumerFactory)
        {
            _logger = logger;
            _consumerFactory = consumerFactory;

            ConnectionFactory factory = GetConfiguredFactory(configuration);

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _logger.LogInformation("---> Connected to RabbitMQ Message Bus : {date}", DateTime.UtcNow);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            BindQueues();

            ConsumeQueues();

            return Task.CompletedTask;
        }
      
        private void BindQueues()
        {
 
            BindQueue(ArticleEXCH, ArticleCreateRK);
            BindQueue(ArticleEXCH, ArticleDeleteRK);
            BindQueue(ArticleEXCH, ArticleUpdateRK);
        }

        private void BindQueue(string exchange, string routingKey)
        {
            _channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);

            string queueName = _channel.QueueDeclare().QueueName;

            _channel.QueueBind(queue: queueName,
                               exchange: exchange,
                               routingKey: routingKey);

            _queues.Add(new Tuple<string, string>(routingKey, queueName));
        }

        private void ConsumeQueues()
        {
            foreach (var tuple in _queues)
            {
                EventingBasicConsumer consumer = _consumerFactory.CreateConsumer(_channel, tuple.Item1);
                _channel.BasicConsume(queue: tuple.Item2, autoAck: true, consumer: consumer);
            }
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
