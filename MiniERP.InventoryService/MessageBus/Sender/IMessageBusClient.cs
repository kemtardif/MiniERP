using MiniERP.InventoryService.MessageBus.Messages;

namespace MiniERP.InventoryService.MessageBus.Sender
{
    public interface IMessageBusClient
    {
        void Publish(GenericEvent evnt);
    }
}
