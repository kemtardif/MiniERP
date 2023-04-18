using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using System.Text;
using MiniERP.ArticleService.MessageBus.Events;
using System.Text.Json;

namespace MiniERP.ArticleService.MessageBus
{
    public class RabbitMQClient : IMessageBusClient
    {
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQClient> _logger;

        public RabbitMQClient(IRabbitMQConnection rabbitMQConnection, ILogger<RabbitMQClient> logger)
        {
            _logger = logger;

            if(!rabbitMQConnection.Connection.IsOpen)
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

        public void PublishNewArticle(GenericEvent dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            string message = JsonSerializer.Serialize(dto);

            PublishMessage(message, dto.EventName, dto.RoutingKey);

        }
        private void PublishMessage(string message, string eventName, string routingKey)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "article",
                            routingKey: routingKey,
                            basicProperties: null,
                            body: body);
            _logger.LogInformation("RabbitMQ : {method} : Message Published :  {event}:{date}", nameof(PublishMessage),
                                                                                        eventName,
                                                                                        DateTime.UtcNow);
        }
    }
}
