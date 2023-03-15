using MiniERP.ArticleService.Dtos;

namespace MiniERP.ArticleService.MessageBus
{
    public interface IMessageBusClient
    {
        void PublishNewArticle(GenericPublishDto dto, string routingKey);
    }
}
