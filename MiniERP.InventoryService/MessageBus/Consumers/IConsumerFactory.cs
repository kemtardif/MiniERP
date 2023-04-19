using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MiniERP.InventoryService.MessageBus.Consumers
{
    public interface IConsumerFactory
    {
        EventingBasicConsumer CreateConsumer(IModel model, string routingKey);
    }
}
