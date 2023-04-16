using System.Text.Json.Serialization;

namespace MiniERP.InventoryService.MessageBus.Messages
{
    [JsonDerivedType(typeof(StockChangedEvent))]
    public class GenericEvent
    {
        public string EventName { get; set; } = string.Empty;
        [JsonIgnore]
        public string RoutingKey { get; set; } = string.Empty;
    }
}
