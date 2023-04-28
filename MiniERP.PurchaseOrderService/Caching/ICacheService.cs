using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Caching
{
    public interface ICacheService
    {
        InventoryItem? GetInventoryById(int id);
    }
}
