using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public interface IStockCache
    {
        IEnumerable<AvailableInventoryView> GetAllActualInventory();
        IEnumerable<PendingInventoryView> GetAllPendingInventory();

        AvailableInventoryView? GetAvailableByArticleId(int articleId);
        PendingInventoryView? GetPendingByArticleId(int articleId);

        void Invalidate(int articleId);
    }
}
