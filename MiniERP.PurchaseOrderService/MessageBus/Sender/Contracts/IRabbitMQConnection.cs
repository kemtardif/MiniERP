using RabbitMQ.Client;

namespace MiniERP.PurchaseOrderService.MessageBus.Sender.Contracts
{
    public interface IRabbitMQConnection
    {
        IConnection Connection { get; }
    }
}
