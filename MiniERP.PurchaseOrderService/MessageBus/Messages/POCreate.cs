namespace MiniERP.PurchaseOrderService.MessageBus.Messages
{
    public class POCreate : MessageBase
    {
        public int Id { get; set; }
        public double Quantity { get; set; }
    }
}
