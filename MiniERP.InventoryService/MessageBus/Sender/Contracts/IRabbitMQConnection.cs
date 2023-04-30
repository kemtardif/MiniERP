using RabbitMQ.Client;

namespace MiniERP.InventoryService.MessageBus.Sender.Contracts
{
    public interface IRabbitMQConnection
    {
        IConnection Connection { get; }
    }
}
