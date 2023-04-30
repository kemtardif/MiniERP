namespace MiniERP.PurchaseOrderService.MessageBus.Messages
{
    public class OrderCancelled : MessageBase
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public OrderCancelled(int id)
        {
            Id = id;
            Type = 2;
            Headers.Add("MessageType", "OrderCancelled");
        }
    }
}
