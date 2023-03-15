using MiniERP.ArticleService.Dtos;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.IO;

namespace MiniERP.ArticleService.MessageBus
{
    public class RabbitMQClient : IMessageBusClient, IDisposable
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
            catch(IOException ex)
            {
                _logger.LogError("----> RabbitMQ Exception: {name} : {exName} : {ex} : {date}",
                                    nameof(Dispose),
                                    nameof(IOException),
                                    ex.Message,
                                    DateTime.UtcNow);
            }          
        }

        public void PublishNewArticle(GenericPublishDto dto, string routingKey)
        {
            if(_connection is null)
            {
                _logger.LogError("----> RabbitMQ : {method} : Invalid connection : {date}", nameof(PublishNewArticle), DateTime.UtcNow);
                return;
            }
            if(!_connection.IsOpen)
            {
                _logger.LogError("----> RabbitMQ : {method} : Closed connection : {date}", nameof(PublishNewArticle), DateTime.UtcNow);
                return;
            }
            string message = string.Empty;
            try
            {
                message = JsonSerializer.Serialize(dto);
            } 
            catch(NotSupportedException ex)
            {
                _logger.LogError("----> RabbitMQ Exception : {name}  : {ex} : {date}", nameof(NotSupportedException),
                                                                                ex.Message, 
                                                                                DateTime.UtcNow);
                return;
            }
            if(string.IsNullOrEmpty(message))
            {
                _logger.LogInformation("RabbitMQ : {method} : Empty Publish message : {date}", nameof(PublishNewArticle), DateTime.UtcNow);
                return;
            }
            PublishMessage(message, routingKey, dto.EventName);

        }
        private void PublishMessage(string message, string routingKey, string eventName)
        {
            if (_channel is null)
            {
                _logger.LogError("----> RabbitMQ : {method} : Invalid chaneel : {date}", nameof(PublishMessage), DateTime.UtcNow);
                return;
            }
            byte[]? body = null;
            try
            {
                body = Encoding.UTF8.GetBytes(message);
            } 
            catch(EncoderFallbackException ex)
            {
                _logger.LogError("----> RabbitMQ Exception : {name}  : {ex} : {date}", nameof(EncoderFallbackException),
                                                                                ex.Message, 
                                                                                DateTime.UtcNow
                                                                                );
                return;
            }
            if(body is null || body.Length == 0)
            {
                _logger.LogInformation("RabbitMQ : {method} : Empty Byte body : {date}", nameof(PublishMessage), DateTime.UtcNow);
                return;
            }
            _channel.BasicPublish(exchange: "article",
                            routingKey: routingKey,
                            basicProperties: null,
                            body: body);
            _logger.LogInformation("RabbitMQ : {method} : Message Published : {route} :  {event}:{date}", nameof(PublishMessage), 
                                                                                        routingKey, 
                                                                                        eventName,
                                                                                        DateTime.UtcNow);
        }
    }
}
