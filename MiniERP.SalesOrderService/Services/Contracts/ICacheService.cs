using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Services.Contracts
{
    public interface ICacheService
    {
        InventoryItem? GetItemById(int articleId);
    }
}
