using MiniERP.InventoryService.MessageBus.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MiniERP.InventoryService.MessageBus.Subscriber
{
    public class RabbitMQConsumer : EventingBasicConsumer
    {
        private readonly ILogger<RabbitMQSubscriber> _logger;
        private readonly IMessageRouter _router;

        public RabbitMQConsumer(ILogger<RabbitMQSubscriber> logger,
                                IMessageRouter router,
                                IModel model) : base(model)
        {
            _logger = logger;
            _router = router;

            Received += ReceivedHandler;
        }

        private void ReceivedHandler(object? consumer, BasicDeliverEventArgs args)
        {
            _logger.LogInformation("----> RabbitMQ : Message Received : {key} : {tag} : {date}",
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
                return;
            }

            IMessage eventMessage = GetMessage(data);

            _router.RouteMessage(eventMessage, data);

            _logger.LogInformation("---> RabbitMQ : Message routed : {tag} : {date}",
                                        args.DeliveryTag,
                                        DateTime.UtcNow);
        }

        private IMessage GetMessage(string message)
        {

            try
            {
                IMessage? eventMessage = JsonSerializer.Deserialize<MessageBase>(message);

                if (eventMessage is null)
                {
                    return new UndefinedMessage();
                }
                return eventMessage;

            }
            catch (JsonException ex)
            {
                _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                    nameof(GetMessage),
                                    nameof(JsonException),
                                    ex.Message,
                                    DateTime.UtcNow);

                return new UndefinedMessage();
            }
        }
    }
}
