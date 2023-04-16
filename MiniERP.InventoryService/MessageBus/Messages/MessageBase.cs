using System.Text.Json.Serialization;

namespace MiniERP.InventoryService.MessageBus.Messages
{
    [JsonDerivedType(typeof(ArticleMessage))]
    public class MessageBase : IMessage
    {
        public string EventName { get; set; } = string.Empty;
    }
}
