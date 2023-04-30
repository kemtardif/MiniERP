namespace MiniERP.InventoryService.MessageBus.Messages
{
    public class OrderClosed : MessageBase
    {
        public int Id { get; set; }
        public int Type { get; set; }
    }
}
