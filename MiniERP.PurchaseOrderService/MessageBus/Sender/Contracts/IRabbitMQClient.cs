using MiniERP.PurchaseOrderService.MessageBus.Messages;

namespace MiniERP.PurchaseOrderService.MessageBus.Sender.Contracts
{
    public interface IRabbitMQClient
    {
        void Publish(MessageBase message);
    }
}
