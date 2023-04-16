namespace MiniERP.InventoryService.MessageBus.Messages
{
    public interface IMessage
    {
        string EventName { get; }
    }
}
