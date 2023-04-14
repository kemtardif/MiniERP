using MiniERP.InventoryService.MessageBus.Events;

namespace MiniERP.InventoryService.MessageBus.Sender
{
    public interface IMessageBusClient
    {
        void Publish(GenericEvent evnt);
    }
}
