using System.Text.Json.Serialization;

namespace MiniERP.ArticleService.MessageBus.Events
{
    [JsonDerivedType(typeof(InventoryEvent))]
    public class GenericEvent
    {
        public string EventName { get; set; } = string.Empty;
        [JsonIgnore]
        public string RoutingKey { get; set; } = string.Empty;
    }
}
