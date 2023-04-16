using Microsoft.Extensions.Caching.Distributed;
using MiniERP.PurchaseOrderService.Grpc;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Caching
{
    public class RedisCacheRepository : ICacheRepository
    {
        private readonly IDistributedCache _cache;
        private readonly IInventoryDataClient _inventory;

        public RedisCacheRepository(IDistributedCache cache,
                                    IInventoryDataClient inventory)
        {
            _cache = cache;
            _inventory = inventory;
        }
        public async Task<Stock?> GetStockByArticleId(int articleId)
        {
            Stock? stock = await _cache.GetRecordAsync<Stock>(articleId.ToString());

            if (stock is null)
            {
                stock = _inventory.GetStockByArticleId(articleId);

                if (stock is not null)
                {
                    await _cache.SetRecordAsync(articleId.ToString(), stock);
                }
            }

            return stock;
        }

        public async Task SetStock(Stock stock)
        {
            await _cache.SetRecordAsync(stock.InventoryId.ToString(), stock);
        }
    }
}
