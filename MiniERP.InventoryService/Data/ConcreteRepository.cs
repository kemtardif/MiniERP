using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{

    public class ConcreteRepository : IRepository
    {
        private readonly AppDbContext _context;
        private readonly ICache _cache;

        private readonly HashSet<int> _invalidates = new();
        public ConcreteRepository(AppDbContext context,
                                   ICache cache)
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

           _invalidates.Add(item.ArticleId);
        }

        public IEnumerable<InventoryItem> GetAllItems()
        {
            return _context.InventoryItems;
        }

        public InventoryItem? GetInventoryByArticleId(int articleId)
        {   
            return _context.InventoryItems.FirstOrDefault(x => x.ArticleId == articleId);
        }

        public void CloseItem(int articleId)
        {
            InventoryItem? item = _context.InventoryItems.FirstOrDefault(x => x.ArticleId == articleId);

            if (item is null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            item.SetAsClosed();

            _invalidates.Add(articleId);

        }

        public void SaveChanges()
        {

            _context.SaveChanges();

            foreach(int invalidate in _invalidates)
            {
                _cache.Invalidate(invalidate);
            }
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

            _invalidates.Add(item.ArticleId);
        }

        public void Update(InventoryMovement item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.StockMovements.Update(item);

            _invalidates.Add(item.ArticleId);
        }
    }
}
