using System.Text.Json.Serialization;

namespace MiniERP.ArticleService.Dtos
{
    [JsonDerivedType(typeof(InventoryPublishDto))]
    public class ArticlePublishDto
    {
        public int Id { get; set; }
        public string EventName { get; set; } = string.Empty;
    }
}
