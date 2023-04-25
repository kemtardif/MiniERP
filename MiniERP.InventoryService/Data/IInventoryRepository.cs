using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public interface IInventoryRepository
    {
        IEnumerable<InventoryItem> GetAllItems();
        InventoryItem? GetInventoryByArticleId(int articleId);
        IEnumerable<InventoryMovement> GetMovementsByOrder(RelatedOrderType orderType, int orderId);
        void AddItem(InventoryMovement item);
        void AddItem(InventoryItem item);
        void SetAsClosed(int articleId);

        void Update(InventoryItem item);
        void SaveChanges();
    }
}
