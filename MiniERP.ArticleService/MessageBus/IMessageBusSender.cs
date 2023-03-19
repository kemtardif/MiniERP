using MiniERP.ArticleService.Data;

namespace MiniERP.ArticleService.MessageBus
{
    public interface IMessageBusSender<T> 
    {
        void RequestForPublish(RequestType type, ChangeType changeType, T item);
    }
    public enum RequestType
    {
        Created,
        Updated,
        Deleted
    }
}
