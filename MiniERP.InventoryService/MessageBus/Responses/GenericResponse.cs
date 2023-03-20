using System.Text.Json.Serialization;

namespace MiniERP.InventoryService.MessageBus.Responses
{
    [JsonDerivedType(typeof(ArticleResponse))]
    public class GenericResponse
    {
        public string EventName { get; set; } = string.Empty;
    }
}
