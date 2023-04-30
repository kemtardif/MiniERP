namespace MiniERP.InventoryService.MessageBus.Messages
{
    public class POCreate : MessageBase
    {
        public int Id { get; }
        public double Quantity { get; }
        public POCreate(int id, double quantity)
        {
            Id = id;
            Quantity = quantity;
            Headers.Add("MessageType", "POCreate");
        }

    }
}
