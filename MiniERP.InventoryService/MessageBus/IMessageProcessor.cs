namespace MiniERP.InventoryService.MessageBus
{
    public interface IMessageProcessor
    {
        Task ProcessMessage(string message);
    }
}
