using System.Collections.Generic;

namespace MiniERP.InventoryService.MessageBus.Sender
{
    public interface IMessageBusSender
    {
        void RequestForPublish<T>(RequestType type, IEnumerable<T> item);
    }
    public enum RequestType
    {
        StockUpdated,
    }
}
