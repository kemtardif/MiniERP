using MiniERP.ArticleService.MessageBus.Messages;
using RabbitMQ.Client;

namespace MiniERP.ArticleService.MessageBus.Sender.Contracts
{
    public interface IMessageBusClient
    {
        void Publish(MessageBase message);
    }

}
