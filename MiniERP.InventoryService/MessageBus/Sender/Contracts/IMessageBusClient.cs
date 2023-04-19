using MiniERP.InventoryService.MessageBus.Messages;

namespace MiniERP.InventoryService.MessageBus.Sender.Contracts
{
    public interface IMessageBusClient
    {
        void Publish(GenericEvent evnt);
    }
}
