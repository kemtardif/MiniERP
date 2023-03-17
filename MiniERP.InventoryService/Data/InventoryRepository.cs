using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.Exceptions;
using MiniERP.InventoryService.Extensions;
using MiniERP.InventoryService.MessageBus.Responses;
using MiniERP.InventoryService.Models;
using System.Runtime.CompilerServices;
using System.Text.Json;

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

            _context.SaveChanges();
        }

        //No cache involved
        public IEnumerable<Stock> GetAllItems()
        {
            return _context.InventoryItems.ToList();
        }

        // GET cache or add if exists but notpresent
        public async Task<Stock?> GetItemByArticleId(int articleId)
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
        public async Task RemoveItem(Stock item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.InventoryItems.Remove(item);

            _context.SaveChanges();

            await _cache.RemoveAsync(item.ProductId.ToString());
        }

        //Update cache if exists
        public async Task SetAsDiscontinued(Stock item)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.SetAsDiscontinued();
            item.SetUpdatedAtToCurrentTime();
            
            _context.SaveChanges();

            await UpdateCacheIfItemExists(item);
        }
        //Update cache if exists
        public async Task UpdateFromMessage(Stock item, ArticleResponse article)
        {
            _mapper.Map(article, item);

            item.SetUpdatedAtToCurrentTime();

            _context.SaveChanges();

           await UpdateCacheIfItemExists(item);
        }
        private async Task  UpdateCacheIfItemExists(Stock item)
        {
            Stock? stock = await _cache.GetRecordAsync<Stock?>(item.ProductId.ToString());
            if (stock is not null)
            {
                _logger.LogInformation("---> CACHE HIT : {method} : {id}", nameof(UpdateCacheIfItemExists), item.ProductId);
                await _cache.SetRecordAsync(item.ProductId.ToString(), item);
                return;
            }
            _logger.LogInformation("---> CACHE MISS : {method} : {id}", nameof(UpdateCacheIfItemExists), item.ProductId);
        }
    }
}
