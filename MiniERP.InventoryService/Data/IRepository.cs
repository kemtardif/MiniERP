using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public interface IRepository
    {
        IEnumerable<InventoryItem> GetAllItems();
        InventoryItem? GetInventoryByArticleId(int articleId);
        AvailableInventoryView? GetAvailableByArticleId(int articleId);
        IEnumerable<InventoryMovement> GetMovementsByOrder(RelatedOrderType orderType, int orderId);
        void AddItem(InventoryMovement item);
        void AddItem(InventoryItem item);
        void CloseItem(int articleId);

        void Update(InventoryItem item);
        void Update(InventoryMovement item);
        Task SaveChanges();
    }
}
