namespace MiniERP.SalesOrderService.MessageBus.Messages
{
    public class OrderClosed : MessageBase
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public OrderClosed(int id)
        {
            Id = id;
            Type = 1;
            Headers.Add("MessageType", "OrderClosed");
        }
    }
}
