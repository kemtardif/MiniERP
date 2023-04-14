using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public interface IInventoryRepository
    {
        IEnumerable<InventoryItem> GetAllItems();
        InventoryItem? GetItemById(int id);
        InventoryItem? GetItemByArticleId(int articleId);
        void AddInventoryItem(InventoryItem item);
        void SetAsDiscontinued(InventoryItem item);
        void SetAsClosed(InventoryItem item);
        void SaveChanges();
    }
}
