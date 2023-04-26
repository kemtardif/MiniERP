namespace MiniERP.InventoryService.MessageBus.Messages
{
    public class OrderCancelled : MessageBase
    {
        public int Id { get; set; }
        public int Type { get; set; }
    }
}
