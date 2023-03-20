using MiniERP.InventoryService.MessageBus.Responses;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public interface IInventoryRepository
    {
        IEnumerable<Stock> GetAllItems();
        Task<Stock?> GetItemById(int id);
        Task<Stock?> GetItemByArticleIdAsync(int articleId);
        Stock? GetItemByArticleIdFromSource(int articleId);
        void AddItem(Stock item);
        void RemoveItem(Stock item);
        void SetAsDiscontinued(Stock item);
        void SetAsClosed(Stock item);
        Task SaveChanges();
        void UpdateFromMessage(Stock item, ArticleResponse article);
    }
}
