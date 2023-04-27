using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Services.Contracts
{
    public interface IRPCService
    {
        InventoryItem? GetItemById(int articleId);
    }
}
