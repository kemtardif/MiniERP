using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using System.Text;
using MiniERP.ArticleService.MessageBus.Sender.Contracts;

namespace MiniERP.ArticleService.MessageBus.Sender
{
    public class RabbitMQClient : IMessageBusClient, IDisposable
    {
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQClient> _logger;

        public RabbitMQClient(IRabbitMQConnection rabbitMQConnection, ILogger<RabbitMQClient> logger)
        {
            _logger = logger;

            if (!rabbitMQConnection.Connection.IsOpen)
            {
                throw new BrokerUnreachableException(new ArgumentNullException(nameof(rabbitMQConnection)));
            }

            _channel = rabbitMQConnection.Connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "article", type: ExchangeType.Direct);

        }

        public void Dispose()
        {
            _channel.Close();
        }

        public void PublishMessage(string routingKey, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }
            byte[] body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "article",
                            routingKey: routingKey,
                            basicProperties: null,
                            body: body);
            _logger.LogInformation("RabbitMQ : Message Published :  {key} : {date}",
                                    routingKey,
                                    DateTime.UtcNow);

        }
    }
}
