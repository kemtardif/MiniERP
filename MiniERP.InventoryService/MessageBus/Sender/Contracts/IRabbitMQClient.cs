using MiniERP.InventoryService.MessageBus.Messages;

namespace MiniERP.InventoryService.MessageBus.Sender.Contracts
{
    public interface IRabbitMQClient
    {
        void Publish(MessageBase message);
    }
}
