using RabbitMQ.Client;

namespace MiniERP.SalesOrderService.MessageBus.Sender.Contracts
{
    public interface IRabbitMQConnection
    {
        IConnection Connection { get; }
    }
}
