using RabbitMQ.Client;

namespace MiniERP.ArticleService.MessageBus
{
    public interface IRabbitMQConnection
    {
        IConnection Connection { get; }
    }
}
