using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Commands;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Models.Views;

namespace MiniERP.InventoryService.Data
{

    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;

        private readonly HashSet<int> _invalidates = new();
        public InventoryRepository(AppDbContext context,
                                  IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public void AddItem(InventoryItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _context.InventoryItems.Add(item);
        }

        public void AddItem(InventoryMovement item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _context.StockMovements.Add(item);

           _invalidates.Add(item.ArticleId);
        }

        public IEnumerable<InventoryItem> GetAllItems()
        {
            return _context.InventoryItems;
        }

        public InventoryItem? GetInventoryById(int articleId)
        {   
            return _context.InventoryItems.FirstOrDefault(x => x.ArticleId == articleId);
        }

        public AvailableInventoryView? GetAvailableByArticleId(int articleId)
        {
            return _context.AvailableInventoryView.FirstOrDefault(x => x.ArticleId == articleId);
        }

        public void CloseItem(int articleId)
        {
            InventoryItem? item = _context.InventoryItems.FirstOrDefault(x => x.ArticleId == articleId);

            if (item is null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            item.SetAsClosed();

            _invalidates.Add(articleId);

        }

        public async Task SaveChanges()
        {

            _context.SaveChanges();

            foreach(int invalidate in _invalidates)
            {
                await _mediator.Send(new InvalidateCacheCommand(invalidate));
            }
            _invalidates.Clear();
        }

        public IEnumerable<InventoryMovement> GetMovementsByOrder(RelatedOrderType orderType, int orderId)
        {
            return _context.StockMovements
                    .Include(x => x.InventoryItem)
                    .Where(x => x.RelatedOrderType == orderType && x.RelatedOrderId == orderId);
        }

        public void Update(InventoryItem item)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.InventoryItems.Update(item);

            _invalidates.Add(item.ArticleId);
        }

        public void Update(InventoryMovement item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.StockMovements.Update(item);

            _invalidates.Add(item.ArticleId);
        }

        public void CloseMovementByOrder(RelatedOrderType orderType, int orderId)
        {
            var items = _context.StockMovements
                                .Include(x => x.InventoryItem)
                                .Where(x => x.RelatedOrderType == orderType && x.RelatedOrderId == orderId)
                                .ToList();

            foreach (var item in items)
            {
                item.MovementStatus = MovementStatus.Closed;
                _invalidates.Add(item.ArticleId);
            }
            _context.StockMovements.UpdateRange(items);
        }

        public void CancelMovementByOrder(RelatedOrderType orderType, int orderId)
        {
            var items = _context.StockMovements
                                .Include(x => x.InventoryItem)
                                .Where(x => x.RelatedOrderType == orderType && x.RelatedOrderId == orderId)
                                .ToList();

            foreach (var item in items)
            {
                item.MovementStatus = MovementStatus.Cancelled;
                _invalidates.Add(item.ArticleId);
            }
            _context.StockMovements.UpdateRange(items);
        }
    }
}
