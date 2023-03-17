using MiniERP.InventoryService.MessageBus.Responses;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public interface IInventoryRepository
    {
        IEnumerable<Stock> GetAllItems();
        Task<Stock?> GetItemById(int id);
        Task<Stock?> GetItemByArticleId(int srticleId);
        void AddItem(Stock item);
        Task RemoveItem(Stock item);
        Task SetAsDiscontinued(Stock item);
        Task UpdateFromMessage(Stock item, ArticleResponse article);
    }
}
