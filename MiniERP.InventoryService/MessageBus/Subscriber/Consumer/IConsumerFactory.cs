using RabbitMQ.Client;

namespace MiniERP.InventoryService.MessageBus.Subscriber.Consumer
{
    public interface IConsumerFactory
    {
        RabbitMQConsumer Create(IModel model);
    }
}
