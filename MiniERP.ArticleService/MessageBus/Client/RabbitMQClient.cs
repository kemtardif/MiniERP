using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using System.Text;
using MiniERP.ArticleService.MessageBus.Sender.Contracts;
using MiniERP.ArticleService.MessageBus.Messages;
using System.Text.Json;

namespace MiniERP.ArticleService.MessageBus.Sender
{
    public class RabbitMQClient : IMessageBusClient, IDisposable
    {
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQClient> _logger;
        private readonly string ARTICLE_EXCHANGE = "article";

        public RabbitMQClient(IRabbitMQConnection rabbitMQConnection, ILogger<RabbitMQClient> logger)
        {
            _logger = logger;

            if (!rabbitMQConnection.Connection.IsOpen)
            {
                throw new BrokerUnreachableException(new ArgumentNullException(nameof(rabbitMQConnection)));
            }

            _channel = rabbitMQConnection.Connection.CreateModel();

            _channel.ExchangeDeclare(exchange: ARTICLE_EXCHANGE, type: ExchangeType.Direct);

        }

        public void Dispose()
        {
            _channel.Close();
        }

        public void Publish(MessageBase message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            string messageStr = JsonSerializer.Serialize(message, message.GetType());

            byte[] body = Encoding.UTF8.GetBytes(messageStr);

            IBasicProperties properties = GetBasicProperties(message);

            _channel.BasicPublish(exchange: ARTICLE_EXCHANGE,
                            routingKey: string.Empty,
                            basicProperties: properties,
                            body: body);

            _logger.LogInformation("RabbitMQ : Article Message Published :  {id} : {type} {date}",
                                    message.Id,
                                    message.GetType(),
                                    DateTime.UtcNow);

        }

        private IBasicProperties GetBasicProperties(MessageBase message)
        {
            IBasicProperties props = _channel.CreateBasicProperties();

            props.Headers = new Dictionary<string, object>();

            foreach(var kvp in message.Headers)
            {
                props.Headers.Add(kvp.Key, kvp.Value);
            }
            return props;
        }
    }
}
