using MediatR;
using RabbitMQ.Client;

namespace MiniERP.PurchaseOrderService.MessageBus.Subscriber.Consumer
{
    public class RabbitMQConsumerFactory : IConsumerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQConsumerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public RabbitMQConsumer Create(IModel model)
        {
            ILogger<RabbitMQConsumer> logger = _serviceProvider.GetRequiredService<ILogger<RabbitMQConsumer>>();
            IMediator mediator = _serviceProvider.GetRequiredService<IMediator>();

            return new RabbitMQConsumer(logger, model, mediator);
        }
    }
}
