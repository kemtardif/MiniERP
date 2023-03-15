using Microsoft.Extensions.Logging;
using MiniERP.ArticleService.Dtos;
using System.Text.Json;
using RabbitMQ.Client;
using System.Threading.Channels;
using RabbitMQ.Client.Exceptions;
using System.Text;

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
                _logger.LogCritical("---> RabbitMQ Exception: {name} : {ex}", nameof(BrokerUnreachableException), 
                                                                              ex.Message);
                return;
            }

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "article", type: ExchangeType.Direct);

            _logger.LogInformation("---> Connected to RabbitMQ Message Bus");
        }    
        public void PublishNewArticle(ArticlePublishDto dto, string routingKey)
        {
            if(_connection is null)
            {
                _logger.LogError("----> RabbitMQ : {method} : Invalid connection", nameof(PublishNewArticle));
                return;
            }
            if(!_connection.IsOpen)
            {
                _logger.LogError("----> RabbitMQ : {method} :Closed connection", nameof(PublishNewArticle));
                return;
            }
            string message = string.Empty;
            try
            {
                message = JsonSerializer.Serialize(dto);
            } 
            catch(NotSupportedException ex)
            {
                _logger.LogError("----> RabbitMQ Exception : {name}  : {ex} ", nameof(NotSupportedException),
                                                                                ex.Message);
                return;
            }
            if(string.IsNullOrEmpty(message))
            {
                _logger.LogInformation("RabbitMQ : {method} : Empty Publish message", nameof(PublishNewArticle));
                return;
            }
            PublishMessage(message, routingKey);

        }
        private void PublishMessage(string message, string routingKey)
        {
            if (_channel is null)
            {
                _logger.LogError("----> RabbitMQ : {method} : Invalid chaneel", nameof(PublishMessage));
                return;
            }
            byte[]? body = null;
            try
            {
                body = Encoding.UTF8.GetBytes(message);
            } 
            catch(EncoderFallbackException ex)
            {
                _logger.LogError("----> RabbitMQ Exception : {name}  : {ex} ", nameof(EncoderFallbackException),
                                                                                ex.Message);
                return;
            }
            if(body is null || body.Length == 0)
            {
                _logger.LogInformation("RabbitMQ : {method} : Empty Byte body", nameof(PublishMessage));
                return;
            }
            _channel.BasicPublish(exchange: "article",
                            routingKey: routingKey,
                            basicProperties: null,
                            body: body);
            _logger.LogInformation("RabbitMQ : {method} : Message Published : {route}", nameof(PublishMessage), 
                                                                                        routingKey);

        }
    }
}
