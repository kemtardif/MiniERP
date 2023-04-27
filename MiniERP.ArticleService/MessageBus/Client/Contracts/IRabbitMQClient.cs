using MiniERP.ArticleService.MessageBus.Messages;

namespace MiniERP.ArticleService.MessageBus.Sender.Contracts
{
    public interface IRabbitMQClient
    {
        void Publish(MessageBase message);
    }

}
