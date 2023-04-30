using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Caching
{
    public interface ICacheService
    {
        InventoryItem? GetItemById(int articleId);
    }
}
