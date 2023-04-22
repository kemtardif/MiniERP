using System.Text.Json.Serialization;

namespace MiniERP.ArticleService.MessageBus.Messages
{
    public class MessageBase 
    {
        public int Id { get; set; }
        [JsonIgnore]
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    }
}
