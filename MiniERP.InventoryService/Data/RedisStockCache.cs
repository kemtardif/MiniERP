using Microsoft.Extensions.Caching.Distributed;
using MiniERP.InventoryService.Caching;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public class RedisStockCache : IStockCache
    {
        private IDistributedCache _cache;
        private AppDbContext _context;

        private const string AVAILABLEKEY = "available";
        private const string PENDINGKEY = "pending";

        public RedisStockCache(IDistributedCache cache,
                               AppDbContext context)
        {
            _cache = cache;
            _context = context;
        }
        public AvailableInventoryView? GetAvailableByArticleId(int articleId)
        {
            AvailableInventoryView? item = _cache.GetRecord<AvailableInventoryView>($"{AVAILABLEKEY}:{articleId}");

            if(item is null)
            {
                item = _context.AvailableInventoryView.FirstOrDefault(x => x.ArticleId == articleId);

                if(item is not null)
                {
                    _cache.SetRecord($"{AVAILABLEKEY}:{articleId}", item);
                }
            }
            return item;
        }

        public IEnumerable<AvailableInventoryView> GetAllActualInventory()
        {
            return _context.AvailableInventoryView;
        }

        public IEnumerable<PendingInventoryView> GetAllPendingInventory()
        {
            return _context.PendingInventoryView;
        }

        public PendingInventoryView? GetPendingByArticleId(int articleId)
        {
            PendingInventoryView? item = _cache.GetRecord<PendingInventoryView>($"{PENDINGKEY}:{articleId}");

            if (item is null)
            {
                item = _context.PendingInventoryView.FirstOrDefault(x => x.ArticleId == articleId);

                if (item is not null)
                {
                    _cache.SetRecord($"{PENDINGKEY}:{articleId}", item);
                }
            }
            return item;
        }

        public void Invalidate(int articleId)
        {
            _cache.Remove($"{AVAILABLEKEY}:{articleId}");
            _cache.Remove($"{PENDINGKEY}:{articleId}");
        }
    }
}
