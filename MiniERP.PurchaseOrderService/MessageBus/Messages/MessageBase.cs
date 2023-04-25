using System.Text.Json.Serialization;

namespace MiniERP.PurchaseOrderService.MessageBus.Messages
{
    public class MessageBase
    {
        [JsonIgnore]
        public Dictionary<string, string> Headers { get; set; } = new();
    }

    public enum MovementType
    {
        In = 1,
        Out = 2
    }
    public enum MovementStatus
    {
        Open = 1,
        Closed = 2,
        Cancelled = 3
    }
    public enum RelatedOrderType
    {
        SO = 1,
        PO = 2
    }
}
