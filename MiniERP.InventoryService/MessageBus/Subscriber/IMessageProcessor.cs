namespace MiniERP.InventoryService.MessageBus.Subscriber
{
    public interface IMessageProcessor
    {
        void ProcessMessage(string message);
    }
}
