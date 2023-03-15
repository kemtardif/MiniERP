using System.Text.Json.Serialization;

namespace MiniERP.ArticleService.Dtos
{
    [JsonDerivedType(typeof(InventoryPublishDto))]
    public class GenericPublishDto
    {
        public string EventName { get; set; } = string.Empty;
    }
}
