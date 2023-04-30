using RabbitMQ.Client;

namespace MiniERP.PurchaseOrderService.MessageBus.Subscriber.Consumer
{
    public interface IConsumerFactory
    {
        RabbitMQConsumer Create(IModel model);
    }
}
