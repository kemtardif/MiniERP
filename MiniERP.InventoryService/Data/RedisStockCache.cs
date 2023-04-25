using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using MiniERP.InventoryService.Caching;
using MiniERP.InventoryService.Models;
using System.Collections.Concurrent;

namespace MiniERP.InventoryService.Data
{
    public class RedisStockCache : IStockCache
    {
        private IDistributedCache _cache;
        private AppDbContext _context;

        private static readonly ConcurrentDictionary<string, ReaderWriterLockSlim> _locks = new();

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
            string key = $"{AVAILABLEKEY}:{articleId}";

            var cacheLock = _locks.GetOrAdd(key, x => new ReaderWriterLockSlim());

            cacheLock.EnterReadLock();
            AvailableInventoryView? item = _cache.GetRecord<AvailableInventoryView>($"{AVAILABLEKEY}:{articleId}");
            cacheLock.ExitReadLock();

            if (item is null)
            {
                item = _context.AvailableInventoryView.FirstOrDefault(x => x.ArticleId == articleId);

                if (item is not null)
                {
                    cacheLock.EnterWriteLock();
                    _cache.SetRecord($"{AVAILABLEKEY}:{articleId}", item);
                    cacheLock.ExitWriteLock();
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
            string key = $"{PENDINGKEY}:{articleId}";

            var cacheLock = _locks.GetOrAdd(key, x => new ReaderWriterLockSlim());

            cacheLock.EnterReadLock();
            PendingInventoryView? item = _cache.GetRecord<PendingInventoryView>($"{PENDINGKEY}:{articleId}");
            cacheLock.ExitReadLock();

            if (item is null)
            {
                item = _context.PendingInventoryView.FirstOrDefault(x => x.ArticleId == articleId);

                if (item is not null)
                {
                    cacheLock.EnterWriteLock();
                    _cache.SetRecord($"{PENDINGKEY}:{articleId}", item);
                    cacheLock.ExitWriteLock();
                }
            }
            return item;
        }

        public void Invalidate(int articleId)
        {
            string available = $"{AVAILABLEKEY}:{articleId}";
            string pending = $"{PENDINGKEY}:{articleId}";

            var availableLock = _locks.GetOrAdd(available, x => new ReaderWriterLockSlim());
            var pendingLock = _locks.GetOrAdd(pending, x => new ReaderWriterLockSlim());

            availableLock.EnterWriteLock();
            _cache.Remove($"{AVAILABLEKEY}:{articleId}");
            availableLock.ExitWriteLock();

            pendingLock.EnterWriteLock();
            _cache.Remove($"{PENDINGKEY}:{articleId}");
            pendingLock.ExitWriteLock();
        }
    }
}
