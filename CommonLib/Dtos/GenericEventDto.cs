using System.Text.Json.Serialization;

namespace CommonLib.Dtos
{
    [JsonDerivedType(typeof(ArticleEventDto))]
    public class GenericEventDto
    {
        public string EventName { get; set; } = string.Empty;
    }
}
