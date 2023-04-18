using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Grpc
{
    public interface IMovementProcessor
    {
        bool Open(InventoryItem item, StockMovement osm, out string error);
        bool Close(List<StockMovement> movements, out string error);
        bool Cancel(List<StockMovement> movements, out string error);
    }
}
