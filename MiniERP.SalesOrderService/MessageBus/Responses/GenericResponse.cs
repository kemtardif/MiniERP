using System.Text.Json.Serialization;

namespace MiniERP.SalesOrderService.MessageBus.Responses
{
    [JsonDerivedType(typeof(StockChangedResponse))]
    public class GenericResponse
    {
        public string EventName { get; set; } = string.Empty;
    }
}
