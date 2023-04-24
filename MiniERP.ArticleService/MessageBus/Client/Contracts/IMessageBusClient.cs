using MiniERP.ArticleService.MessageBus.Messages;

namespace MiniERP.ArticleService.MessageBus.Sender.Contracts
{
    public interface IMessageBusClient
    {
        void Publish(MessageBase message);
    }

}
