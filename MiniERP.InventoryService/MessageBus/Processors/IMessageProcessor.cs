namespace MiniERP.InventoryService.MessageBus.Processors
{
    public interface IMessageProcessor
    {
        string ServiceType { get; }
        void ProcessMessage(string data);
    }
}
