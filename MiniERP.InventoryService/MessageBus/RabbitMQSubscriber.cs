using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace MiniERP.InventoryService.MessageBus
{
    public class RabbitMQSubscriber : BackgroundService
    {
        private readonly ILogger<RabbitMQSubscriber> _logger;
        private readonly IMessageProcessor _processor;
        private readonly IConnection? _connection;
        private readonly IModel? _channel;
        private readonly string _queueName = string.Empty;
        public RabbitMQSubscriber(IConfiguration configuration, 
                                  ILogger<RabbitMQSubscriber> logger,
                                  IMessageProcessor processor)
        {
            _logger = logger;
            _processor = processor;

            ConnectionFactory factory = InitConnectionFactory(configuration);

            try
            {
                _connection = factory.CreateConnection();
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogCritical("---> RabbitMQ Exception: {name} : {ex} : {date}", nameof(BrokerUnreachableException),
                                                                              ex.Message, DateTime.UtcNow);
                return;
            }

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "article", type: ExchangeType.Direct);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName, 
                               exchange: "article", 
                               routingKey: "inventory");

            _logger.LogInformation("---> Connected to RabbitMQ Message Bus : {date}", DateTime.UtcNow);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += ReceivedHandler;

            _channel?.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }
        private void ReceivedHandler(object? consumer, BasicDeliverEventArgs args)
        {
            _logger.LogInformation("----> RabbitMQ : Message Received : {key} : {tag} :{date}",
                                        args.RoutingKey,
                                        args.DeliveryTag,
                                        DateTime.UtcNow);

            //Decoderfallback shouldn't happen since we use UTF8 always
            string data = Encoding.UTF8.GetString(args.Body.ToArray());

            if (string.IsNullOrEmpty(data))
            {
                _logger.LogInformation("---> RabbitMQ : No received data : {tag} : {date}",
                                        args.DeliveryTag,
                                        DateTime.UtcNow);
                _channel?.BasicAck(args.DeliveryTag, false);
                return;
            }
            _processor.ProcessMessage(data);

            _channel?.BasicAck(args.DeliveryTag, false);

            _logger.LogInformation("---> RabbitMQ : Message Processed : {tag} : {date}",
                                        args.DeliveryTag,
                                        DateTime.UtcNow);
        }
        private ConnectionFactory InitConnectionFactory(IConfiguration configuration)
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

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();

            base.Dispose();
        }
    }
}
