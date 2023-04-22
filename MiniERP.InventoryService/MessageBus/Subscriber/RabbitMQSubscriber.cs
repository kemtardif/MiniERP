﻿using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using MiniERP.InventoryService.MessageBus.Subscriber.Consumer;

namespace MiniERP.InventoryService.MessageBus.Subscriber
{
    public class RabbitMQSubscriber : BackgroundService
    {
        private readonly ILogger<RabbitMQSubscriber> _logger;
        private readonly IConsumerFactory _consumerFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private const string Article_EXCHANGE = "article";

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

            string articleQueue = BindQueue(Article_EXCHANGE, string.Empty);
            ConsumeQueue(articleQueue);

            return Task.CompletedTask;
        }
      

        private string BindQueue(string exchange, string routingKey)
        {
            _channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);

            string queueName = _channel.QueueDeclare().QueueName;

            _channel.QueueBind(queue: queueName,
                               exchange: exchange,
                               routingKey: routingKey);
            return queueName;

        }

        private void ConsumeQueue(string queueName)
        {
            _channel.BasicConsume(queueName, autoAck: true, consumer: _consumerFactory.Create(_channel));
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
