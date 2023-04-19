using MiniERP.ArticleService.MessageBus.Messages;

namespace MiniERP.ArticleService.MessageBus
{
    public interface IMessageBusClient
    {
        void PublishMessage(string routingKey, string message);
    }

}
