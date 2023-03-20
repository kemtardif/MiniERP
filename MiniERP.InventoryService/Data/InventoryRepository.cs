using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MiniERP.InventoryService.Extensions;
using MiniERP.InventoryService.MessageBus.Responses;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly ILogger<InventoryRepository> _logger;

        public InventoryRepository(AppDbContext context, 
                                    IMapper mapper,
                                    IDistributedCache cache,
                                    ILogger<InventoryRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        //Do not add to cache
        public void AddItem(Stock item)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            item.SetCreatedAdToCurrentTime();
            item.SetUpdatedAtToCurrentTime();

            _context.InventoryItems.Add(item);

        }

        //No cache involved
        public IEnumerable<Stock> GetAllItems()
        {
            return _context.InventoryItems.ToList();
        }

        // GET cache or add if exists but notpresent
        public async Task<Stock?> GetItemByArticleIdAsync(int articleId)
        {
            Stock? stock = await _cache.GetRecordAsync<Stock?>(articleId.ToString());
            if (stock is not null)
            {
                return stock;
            }

            stock = _context.InventoryItems.FirstOrDefault(x => x.ProductId == articleId);
            if (stock is not null)
            {
                await _cache.SetRecordAsync(stock.ProductId.ToString(), stock);

            }
            return stock;
        }

        //Add to cache
        public async Task<Stock?> GetItemById(int id)
        {
            Stock? stock = _context.InventoryItems.FirstOrDefault(x => x.Id == id);
            if(stock is not null)
            {
                await _cache.SetRecordAsync(stock.ProductId.ToString(), stock);
            }
            return stock;
        }

        //Remove from cache
        public void RemoveItem(Stock item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.InventoryItems.Remove(item);
        }

        //Update cache if exists
        public void SetAsDiscontinued(Stock item)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.SetAsDiscontinued();
            item.SetUpdatedAtToCurrentTime();
        }
        public void SetAsClosed(Stock item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.SetAsClosed();
            item.SetUpdatedAtToCurrentTime();
        }
        //Update cache if exists
        public void UpdateFromMessage(Stock item, ArticleResponse article)
        {
            _mapper.Map(article, item);

            item.SetUpdatedAtToCurrentTime();
        }
        public async Task SaveChanges()
        {
            List<int> modifiedList = GetChanges();

            if(_context.SaveChanges() > 0)
            {
                await InvalidateCacheChanges(modifiedList);
            }
        }
        public Stock? GetItemByArticleIdFromSource(int articleId)
        {
           return _context.InventoryItems.FirstOrDefault(x => x.ProductId == articleId);
        }
        private List<int> GetChanges()
        {
            List<int> modifiedList = new();

            foreach (var entity in _context.ChangeTracker.Entries<Stock>())
            {
                if (entity.State != EntityState.Unchanged
                    || entity.State != EntityState.Added)
                {
                    modifiedList.Add(entity.Entity.ProductId);
                }
            }
            return modifiedList;
        }
        private async Task InvalidateCacheChanges(List<int> changes)
        {
            foreach(int id in changes)
            {
                await _cache.RemoveAsync(id.ToString());
            }
        }
    }
}
