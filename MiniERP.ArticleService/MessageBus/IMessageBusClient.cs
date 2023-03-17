

using MiniERP.ArticleService.MessageBus.Events;

namespace MiniERP.ArticleService.MessageBus
{
    public interface IMessageBusClient
    {
        void PublishNewArticle(GenericEvent dto);
    }
}
