using Microsoft.Extensions.Caching.Distributed;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Extensions;
using MiniERP.InventoryService.Models.Views;

namespace MiniERP.InventoryService.Caching
{
    public class RedisCacheService : ICacheService
    {
        private const string CacheHitLogFormat = "------> REDIS : Cache {value} for {id}";
        private const string InvalidationLogFormat = "------> REDIS : Cache INVALIDATION for {id}";

        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        private AppDbContext _context;
        public RedisCacheService(IDistributedCache cache,
                          AppDbContext context,
                          ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _context = context;
            _logger = logger;
        }
        public AvailableInventoryView? GetById(int articleId)
        {

            AvailableInventoryView? item = _cache.GetRecord<AvailableInventoryView>(articleId.ToString());

            _logger.LogInformation(CacheHitLogFormat,
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
            _logger.LogInformation(InvalidationLogFormat, articleId);
        }

    }
}
