using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public interface IInventoryRepository
    {
        IEnumerable<InventoryItem> GetAllItems();
        InventoryItem? GetItemById(int id);
        InventoryItem? GetItemByArticleId(int articleId, bool WithStock, bool WithMovement);
        void AddInventoryItem(InventoryItem item);
        void SetAsDiscontinued(InventoryItem item);
        void SetAsClosed(InventoryItem item);
        void SaveChanges();
    }
}
