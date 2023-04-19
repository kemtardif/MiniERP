namespace MiniERP.InventoryService.MessageBus.Sender.Contracts
{
    public interface IMessageBusSender
    {
        void RequestForPublish<T>(RequestType type, IEnumerable<T> item);
    }
    public enum RequestType
    {
        StockUpdated,
    }
}
