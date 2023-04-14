using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using System.Text;
using MiniERP.ArticleService.MessageBus.Events;
using System.Text.Json;

namespace MiniERP.ArticleService.MessageBus
{
    public class RabbitMQClient : IMessageBusClient
    {
        private readonly IConnection? _connection;
        private readonly IModel? _channel;
        private readonly ILogger<RabbitMQClient> _logger;

        public RabbitMQClient(IConfiguration configuration, ILogger<RabbitMQClient> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQHost"],
                Port = int.Parse(configuration["RabbitMQPort"]!),
                UserName = configuration["RabbitMQUser"],
                Password = configuration["RabbitMQPassword"],
                VirtualHost = "/",

            };

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

            _logger.LogInformation("---> Connected to RabbitMQ Message Bus : {date}", DateTime.UtcNow);
        }

        public void Dispose()
        {
            _channel?.Close();
            try
            {
                _connection?.Close();
            }
            catch (IOException ex)
            {
                _logger.LogCritical("----> RabbitMQ Exception: {name} : {exName} : {ex} : {date}",
                                    nameof(Dispose),
                                    nameof(IOException),
                                    ex.Message,
                                    DateTime.UtcNow);
            }
        }

        public void PublishNewArticle(GenericEvent dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (_connection is null || !_connection.IsOpen)
            {
                _logger.LogError("----> RabbitMQ : {method} : Invalid connection : Is null {null} : {isOpen} : {date}",
                    nameof(PublishNewArticle),
                    _connection is null,
                    _connection?.IsOpen,
                    DateTime.UtcNow);
                return;
            }

            string message = JsonSerializer.Serialize(dto);

            PublishMessage(message, dto.EventName, dto.RoutingKey);

        }
        private void PublishMessage(string message, string eventName, string routingKey)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);

            _channel?.BasicPublish(exchange: "article",
                            routingKey: routingKey,
                            basicProperties: null,
                            body: body);
            _logger.LogInformation("RabbitMQ : {method} : Message Published :  {event}:{date}", nameof(PublishMessage),
                                                                                        eventName,
                                                                                        DateTime.UtcNow);
        }
    }
}
