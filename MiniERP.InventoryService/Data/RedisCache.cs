using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using MiniERP.InventoryService.Caching;
using MiniERP.InventoryService.Models;
using System.Collections.Concurrent;

namespace MiniERP.InventoryService.Data
{
    public class RedisCache : ICache
    {
        private readonly IDistributedCache _cache;
        private AppDbContext _context;
        private readonly ILogger<RedisCache> _logger;

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

            if (item is null)
            {
                _logger.LogInformation("------> REDIS : Cache MISS for {id}", articleId);
                item = _context.AvailableInventoryView.FirstOrDefault(x => x.ArticleId == articleId);

                if (item is not null)
                {
                    _cache.SetRecord(articleId.ToString(), item);
                }
            }
            else
            {
                _logger.LogInformation("------> REDIS : Cache HIT for {id}", articleId);
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
