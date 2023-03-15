using MiniERP.ArticleService.Dtos;

namespace MiniERP.ArticleService.MessageBus
{
    public interface IMessageBusClient
    {
        void PublishNewArticle(ArticlePublishDto dto, string routingKey);
    }
}
