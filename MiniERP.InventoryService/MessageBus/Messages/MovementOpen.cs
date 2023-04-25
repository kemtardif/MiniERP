using RabbitMQ.Client;

namespace MiniERP.InventoryService.MessageBus.Messages
{
    public class MovementOpen : MessageBase
    {
        public int RelatedOrderId { get; set; }
        public int RelatedOrderType { get; set; }
        public List<MovementOpenItem> MovementItems { get; set; } = new();
    }
    public class MovementOpenItem
    {
        public int ArticleId { get; set; }
        public int MovementType { get; set; }
        public double Quantity { get; set; }

    }
}
