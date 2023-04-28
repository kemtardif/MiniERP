using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Grpc
{
    public interface IRPCService
    {
        InventoryItem? GetInventoryById(int id);

    }
}
