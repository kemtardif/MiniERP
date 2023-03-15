using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Exceptions;
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
        public void AddItem(Stock item)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _context.InventoryItems.Add(item);
        }

        public IEnumerable<Stock> GetAllItems()
        {
            return _context.InventoryItems.ToList();
        }

        public Stock? GetItemByArticleId(int articleId)
        {
            return _context.InventoryItems.FirstOrDefault(x => x.ProductId == articleId);
        }

        public Stock? GetItemById(int id)
        {
            return _context.InventoryItems.FirstOrDefault(x => x.Id == id);
        }

        public void RemoveItem(Stock item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _context.InventoryItems.Remove(item);
        }

        public bool SaveChanges()
        {
            try
            {
                return _context.SaveChanges() >= 0;
            }
            catch (DbUpdateException updEx)
            {
                throw new SaveChangesException(typeof(Stock), updEx.Message);
            }
        }
        public void SetAsDiscontinued(Stock item)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.AutoQuantity = 0;
            item.MaxQuantity = 0;
            item.AutoTreshold = 0;
            item.AutoOrder = false;
            item.Discontinued = true;
        }
    }
}
