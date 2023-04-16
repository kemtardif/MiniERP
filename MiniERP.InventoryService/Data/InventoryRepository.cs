using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddInventoryItem(InventoryItem item)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.SetCreatedAdToCurrentTime();
            item.SetUpdatedAtToCurrentTime();

            item.Stock.SetCreatedAdToCurrentTime();
            item.Stock.SetUpdatedAtToCurrentTime();

            _context.InventoryItems.Add(item);

        }

        public IEnumerable<InventoryItem> GetAllItems()
        {
            return _context.InventoryItems.Include(x => x.Stock);
        }

        public InventoryItem? GetItemByArticleId(int articleId, bool WithStock, bool WithMovement)
        {
            var items = _context.InventoryItems;

            if(WithStock)
                items.Include(x => x.Stock);
            if (WithMovement)
                items.Include(x => x.StockMovements);

            return items.FirstOrDefault(x => x.ProductId == articleId);
        }

        public InventoryItem? GetItemById(int id)
        {
            return _context.InventoryItems
                    .Include(x => x.Stock)
                    .FirstOrDefault(x => x.Id == id);
            }

        public void SetAsDiscontinued(InventoryItem item)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.SetAsDiscontinued();
            item.SetUpdatedAtToCurrentTime();

        }
        public void SetAsClosed(InventoryItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.SetAsClosed();
            item.SetUpdatedAtToCurrentTime();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
