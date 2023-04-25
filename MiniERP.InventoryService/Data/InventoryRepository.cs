using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{

    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;
        private readonly IStockCache _cache;

        public InventoryRepository(AppDbContext context,
                                   IStockCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public void AddItem(InventoryItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _context.InventoryItems.Add(item);
        }

        public void AddItem(InventoryMovement item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _context.StockMovements.Add(item);

            _cache.Invalidate(item.ArticleId);
        }

        public IEnumerable<InventoryItem> GetAllItems()
        {
            return _context.InventoryItems;
        }

        public InventoryItem? GetInventoryByArticleId(int articleId)
        {   
            return _context.InventoryItems.FirstOrDefault(x => x.ArticleId == articleId);
        }

        public void SetAsClosed(int articleId)
        {
            InventoryItem? item = _context.InventoryItems.FirstOrDefault(x => x.ArticleId == articleId);

            if (item is null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            item.SetAsClosed();

            _cache.Invalidate(articleId);

        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public IEnumerable<InventoryMovement> GetMovementsByOrder(RelatedOrderType orderType, int orderId)
        {
            return _context.StockMovements
                    .Include(x => x.InventoryItem)
                    .Where(x => x.RelatedOrderType == orderType 
                                                    && x.RelatedOrderId == orderId);
        }

        public void Update(InventoryItem item)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.InventoryItems.Update(item);

            _cache.Invalidate(item.ArticleId);
        }          
    }
}
