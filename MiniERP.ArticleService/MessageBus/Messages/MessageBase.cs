using System.Text.Json.Serialization;

namespace MiniERP.ArticleService.MessageBus.Messages
{
    public class MessageBase 
    {        
        [JsonIgnore]
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    }
}
