using RabbitMQ.Client.Events;
using System.Text;
using RabbitMQ.Client;
using MiniERP.InventoryService.MessageBus.Consumers.ConsumerHandlers;

namespace MiniERP.InventoryService.MessageBus.Consumers
{
    public class RabbitMQConsumer : EventingBasicConsumer
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IConsumerHandler _handler;

        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger,
                                IModel model,
                                IConsumerHandler handler) : base(model)
        {
            _logger = logger;
            _handler = handler;

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
                _logger.LogInformation("---> RabbitMQ : No received data : {key} : {tag} : {date}",
                                        args.RoutingKey,
                                        args.DeliveryTag,
                                        DateTime.UtcNow);
                return;
            }

             _handler.Handle(data);

            _logger.LogInformation("---> RabbitMQ : Message Handled : {key} : {tag} : {date}",
                                        args.RoutingKey,    
                                        args.DeliveryTag,
                                        DateTime.UtcNow);
        }
    }
}
