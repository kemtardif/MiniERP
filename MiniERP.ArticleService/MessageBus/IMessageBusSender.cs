using MiniERP.ArticleService.Data;

namespace MiniERP.ArticleService.MessageBus
{
    public interface IMessageBusSender<T> 
    {
        void RequestForPublish(RequestType type, T item, ChangeType changeType);
    }
    public enum RequestType
    {
        Created,
        Updated,
        Deleted
    }
}
