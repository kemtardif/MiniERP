using CommonLib.Dtos;

namespace MiniERP.ArticleService.MessageBus
{
    public interface IMessageBusClient
    {
        void PublishNewArticle(GenericEventDto dto);
    }
}
