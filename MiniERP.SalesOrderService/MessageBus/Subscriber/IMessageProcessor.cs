namespace MiniERP.SalesOrderService.MessageBus
{
    public interface IMessageProcessor
    {
        Task ProcessMessage(string message);
    }
    public enum EventType
    {
        StockUpdated,
        Undefined
    }
}
