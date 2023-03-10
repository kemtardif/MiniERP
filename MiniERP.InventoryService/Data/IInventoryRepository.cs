using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public interface IInventoryRepository
    {
        bool SaveChanges();
        IEnumerable<Stock> GetAllItems();
        Stock? GetItemById(int id);
        Stock? GetItemByArticleId(int srticleId);
        void AddItem(Stock item);
        void RemoveItem(Stock item);
    }
}
