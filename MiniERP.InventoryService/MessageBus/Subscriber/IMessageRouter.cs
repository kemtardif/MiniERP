using MiniERP.InventoryService.MessageBus.Messages;

namespace MiniERP.InventoryService.MessageBus.Subscriber
{
    public interface IMessageRouter
    {
        void RouteMessage(IMessage message, string data);
    }
}
