using MediatR;
using System.Text.Json.Serialization;

namespace MiniERP.InventoryService.MessageBus.Messages
{
    public class MessageBase : IRequest
    {
        [JsonIgnore]
        public Dictionary<string, string> Headers { get; set; } = new();
    }
}
