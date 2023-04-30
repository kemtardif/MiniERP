using MiniERP.SalesOrderService.MessageBus.Messages;

namespace MiniERP.SalesOrderService.MessageBus.Sender.Contracts
{
    public interface IRabbitMQClient
    {
        void Publish(MessageBase message);
    }
}
