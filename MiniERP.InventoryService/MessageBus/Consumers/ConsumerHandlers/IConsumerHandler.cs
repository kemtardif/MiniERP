namespace MiniERP.InventoryService.MessageBus.Consumers.ConsumerHandlers
{
    public interface IConsumerHandler
    {
        void Handle(string message);
    }
}
