using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MiniERP.InventoryService.Caching;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;

        public InventoryRepository(AppDbContext context,
                                   IDistributedCache cache,
                                   IMapper mapper)
        {
            _context = context;
            _cache = cache;
            _mapper = mapper;
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
            InventoryItem? item = _cache.GetRecord<InventoryItem>(articleId.ToString());

            if(item is null)
            {
                item = _context.InventoryItems.FirstOrDefault(x => x.ArticleId == articleId);
                if(item is not null)
                {
                    _cache.SetRecord(articleId.ToString(), item);
                }
            }

            return item;
        }

        public void SetAsClosed(int articleId)
        {
            InventoryItem? item = _context.InventoryItems.FirstOrDefault(x => x.ArticleId == articleId);

            if (item is null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            item.SetAsClosed();

            _cache.Remove(articleId.ToString());
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

            _cache.Remove(articleId.ToString());
        }          
    }
}
