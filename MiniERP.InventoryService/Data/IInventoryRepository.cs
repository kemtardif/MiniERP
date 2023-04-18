using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public interface IInventoryRepository
    {
        IEnumerable<InventoryItem> GetAllItems();
        InventoryItem? GetItemByArticleId(int articleId, bool WithStock, bool WithMovement);
        IEnumerable<StockMovement> GetMovementsByOrder(RelatedOrderType orderType, int orderId);
        void AddInventoryItem(InventoryItem item);
        void SetAsDiscontinued(InventoryItem item);
        void SetAsClosed(InventoryItem item);
        void SaveChanges();
    }
}
