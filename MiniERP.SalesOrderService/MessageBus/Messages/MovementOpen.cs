using System.Data.Common;
using System.Text.Json.Serialization;

namespace MiniERP.SalesOrderService.MessageBus.Messages
{
    public class MovementOpen : MessageBase
    {
        public int RelatedOrderId { get; set; }
        public RelatedOrderType RelatedOrderType { get; set; }

        public List<MovementOpenItem> MovementItems { get; set; } = new();

        public MovementOpen()
        {
            Headers.Add("MessageType", "MovementOpen");
        }
    }
    public class MovementOpenItem
    {
        public int ArticleId { get; set; }
        public MovementType MovementType { get; set; }
        public double Quantity { get; set; }

    }
}
