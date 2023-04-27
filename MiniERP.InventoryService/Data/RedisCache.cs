using Microsoft.Extensions.Caching.Distributed;
using MiniERP.InventoryService.Caching;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public class RedisCache : ICache
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCache> _logger;
        private AppDbContext _context;

        public RedisCache(IDistributedCache cache,
                          AppDbContext context,
                          ILogger<RedisCache> logger)
        {
            _cache = cache;
            _context = context;
            _logger = logger;
        }
        public AvailableInventoryView? GetById(int articleId)
        {

            AvailableInventoryView? item = _cache.GetRecord<AvailableInventoryView>(articleId.ToString());

            _logger.LogInformation("------> REDIS : Cache {value} for {id}",
                                        item is null ? "MISS" : "HIT",
                                        articleId);
            if (item is null)
            {
                item = _context.AvailableInventoryView.FirstOrDefault(x => x.ArticleId == articleId);

                if (item is not null)
                {
                    _cache.SetRecord(articleId.ToString(), item);
                }
            }

            return item;
        }

        public void Invalidate(int articleId)
        {

            _cache.Remove(articleId.ToString());
            _logger.LogInformation("------> REDIS : Cache INVALIDATION for {id}", articleId);
        }

    }
}
