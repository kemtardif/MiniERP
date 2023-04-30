using RabbitMQ.Client.Events;
using System.Text;
using RabbitMQ.Client;
using MediatR;
using System.Text.Json;

namespace MiniERP.PurchaseOrderService.MessageBus.Subscriber.Consumer
{
    public class RabbitMQConsumer : EventingBasicConsumer
    {
        private const string ReceivedLogFormat = "----> RabbitMQ : Message Received : {tag} : {date}";
        private const string NoHeaderLogFormat = "----> RabbitMQ : No header present to route message : {tag} : {date}";
        private const string NoHandlerLogFormat = "----> RabbitMQ : Could not find handler for message type : {header} : {key} : {date}";

        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IMediator _mediator;

        private readonly string MESSAGE_TYPE = "MessageType";
        private readonly string MESSAgE_NAMESPACE = "MiniERP.PurchaseOrderService.MessageBus.Messages";

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
            _logger.LogInformation(ReceivedLogFormat,
                                        args.DeliveryTag,
                                        DateTime.UtcNow);

            if(!AreHeaderPresent(args))
            {
                _logger.LogWarning(NoHeaderLogFormat,
                                        args.DeliveryTag,
                                        DateTime.UtcNow);
                return;
            }

            string header = GetHeaderString(args, MESSAGE_TYPE);

            Type? type = GetMessageType(header);
            if(type is null)
            {
                _logger.LogWarning(NoHandlerLogFormat,
                                        header, 
                                        args.DeliveryTag,
                                        DateTime.UtcNow);
                return;
            }

            Handle(args, type);           
        }
        private bool AreHeaderPresent(BasicDeliverEventArgs args)
        {
            return args.BasicProperties.IsHeadersPresent() && args.BasicProperties.Headers.ContainsKey(MESSAGE_TYPE);
        }

        private string GetHeaderString(BasicDeliverEventArgs args, string key)
        {
            byte[] messageTypeByte = (byte[])args.BasicProperties.Headers[key];

            return Encoding.UTF8.GetString(messageTypeByte);
        }
        private Type? GetMessageType(string type)
        {
            return  Type.GetType(string.Format("{0}.{1}", MESSAgE_NAMESPACE, type));
        }

        private void Handle(BasicDeliverEventArgs args, Type type)
        {
            string data = Encoding.UTF8.GetString(args.Body.ToArray());

            var message = (IRequest)JsonSerializer.Deserialize(data, type)!;

            _mediator.Send(message);
        }
    }
}
