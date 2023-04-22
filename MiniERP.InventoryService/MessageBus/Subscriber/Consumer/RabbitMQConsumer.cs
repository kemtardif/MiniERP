using RabbitMQ.Client.Events;
using System.Text;
using RabbitMQ.Client;
using MediatR;
using System.Text.Json;

namespace MiniERP.InventoryService.MessageBus.Subscriber.Consumer
{
    public class RabbitMQConsumer : EventingBasicConsumer
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IMediator _mediator;

        private readonly string MESSAGE_TYPE = "MessageType";
        private readonly string MESSAgE_NAMESPACE = "MiniERP.InventoryService.MessageBus.Messages";

        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger,
                                IModel model,
                                IMediator mediator) : base(model)
        {
            _logger = logger;
            _mediator = mediator;

            Received += ReceivedHandler;
        }

        private void ReceivedHandler(object? consumer, BasicDeliverEventArgs args)
        {
            _logger.LogInformation("----> RabbitMQ : Message Received : {tag} : {date}",
                                        args.DeliveryTag,
                                        DateTime.UtcNow);

            if(!AreHeaderPresent(args))
            {
                _logger.LogWarning("----> RabbitMQ : No header present to route message : {tag} : {date}",
                                        args.DeliveryTag,
                                        DateTime.UtcNow);
                return;
            }

            string header = GetStringHeader(args, MESSAGE_TYPE);

            Type? type = GetMessageType(header);
            if(type is null)
            {
                _logger.LogWarning("----> RabbitMQ : Could not find handler for message type : {header} : {key} : {date}",
                                        header, 
                                        args.DeliveryTag,
                                        DateTime.UtcNow);
                return;
            }

            //Decoderfallback shouldn't happen since we use UTF8 always
            string data = Encoding.UTF8.GetString(args.Body.ToArray());

            var message = (IRequest)JsonSerializer.Deserialize(data, type)!;

            _mediator.Send(message);

            _logger.LogInformation("---> RabbitMQ : Message Handled : {key} : {tag} : {date}",
                                        args.RoutingKey,    
                                        args.DeliveryTag,
                                        DateTime.UtcNow);
        }
        private bool AreHeaderPresent(BasicDeliverEventArgs args)
        {
            return args.BasicProperties.IsHeadersPresent() && args.BasicProperties.Headers.ContainsKey(MESSAGE_TYPE);
        }

        private string GetStringHeader(BasicDeliverEventArgs args, string key)
        {
            byte[] messageTypeByte = (byte[])args.BasicProperties.Headers[MESSAGE_TYPE];

            if (messageTypeByte == null)
            {
                return string.Empty;
            }

           return Encoding.UTF8.GetString(messageTypeByte);
        }
        private Type? GetMessageType(string type)
        {
            return  Type.GetType(string.Format("{0}.{1}", MESSAgE_NAMESPACE, type));
        }
    }
}
