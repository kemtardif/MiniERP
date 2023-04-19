namespace MiniERP.ArticleService.MessageBus.Sender.Contracts
{
    public interface IMessageBusSender<T>
    {
        void RequestForPublish(RequestType type, T item);
    }
    public enum RequestType
    {
        Created,
        Updated,
        Deleted
    }
}
