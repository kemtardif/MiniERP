using RabbitMQ.Client;

namespace MiniERP.ArticleService.MessageBus.Sender.Contracts
{
    public interface IRabbitMQConnection
    {
        IConnection Connection { get; }
    }
}
