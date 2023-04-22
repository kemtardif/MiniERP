using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{

    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStockCache _cache;

        public InventoryRepository(AppDbContext context,
                                   IMapper mapper,
                                   IStockCache cache)
        {
            _context = context;
            _mapper = mapper;
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

        public void Update(int articleId, InventoryItemUpdate update)
        {
            InventoryItem? item = _context.InventoryItems.FirstOrDefault(x => x.ArticleId == articleId);

            if(item is null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            _mapper.Map(update, item);

            _cache.Invalidate(articleId);
        }          
    }
}
