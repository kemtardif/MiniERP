using RabbitMQ.Client;

namespace MiniERP.InventoryService.MessageBus.Sender
{
    public interface IRabbitMQConnection
    {
        IConnection Connection { get; }
    }
}
