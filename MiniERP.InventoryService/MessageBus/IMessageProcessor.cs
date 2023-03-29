namespace MiniERP.InventoryService.MessageBus
{
    public interface IMessageProcessor
    {
        void ProcessMessage(string message);
    }
}
