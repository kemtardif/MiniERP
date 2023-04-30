using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Grpc
{
    public interface IRPCService
    {
        InventoryItem? GetItemById(int articleId);
    }
}
