using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Commands;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public class MovementRepository 
    {
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;

        private readonly HashSet<int> _invalidates = new();
        public MovementRepository(AppDbContext context,
                                  IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
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


        public void CancelByOrder(RelatedOrderType orderType, int orderId)
        {
           var items =  _context.StockMovements
                                .Include(x => x.InventoryItem)
                                .Where(x => x.RelatedOrderType == orderType && x.RelatedOrderId == orderId)
                                .ToList();

            foreach(var item in items)
            {
                item.MovementStatus = MovementStatus.Cancelled;
                _invalidates.Add(item.ArticleId);
            }
            _context.StockMovements.UpdateRange(items);
        }
        public void CloseByOrder(RelatedOrderType orderType, int orderId)
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

        //public Stock? GetAvailableById(int id)
        //{
        //    var item = from it in _context.InventoryItems
        //               join mv in _context.StockMovements
        //               on it equals mv.InventoryItem into joined
        //               from j in joined.DefaultIfEmpty()
        //               where j.ArticleId == id
        //               select new JoinedInventory()
        //               {
        //                   ArticleId = it.ArticleId,
        //                   Status = it.Status,
        //                   MovementType = j.MovementType,
        //                   Quantity = j.Quantity,
        //                   MovementStatus = j.MovementStatus
        //               } into grp
        //               group grp by new { grp.ArticleId, grp.Status } into grp1
        //               select new Stock()
        //               {
        //                   ArticleId = grp1.Key.ArticleId,
        //                   Status = grp1.Key.Status,
        //                   Quantity = grp1.Sum(x => GetQuantity(x))
        //               };

        //    return item.FirstOrDefault();
        //}

        //public IEnumerable<Stock> GetAvailableStock()
        //{

        //    return from it in _context.InventoryItems
        //           join mv in _context.StockMovements
        //           on it equals mv.InventoryItem into joined
        //           from j in joined.DefaultIfEmpty()
        //           select new JoinedInventory()
        //           { 
        //               ArticleId = it.ArticleId, 
        //               Status = it.Status, 
        //               MovementType = j.MovementType, 
        //               Quantity = j.Quantity,
        //               MovementStatus = j.MovementStatus
        //           } into grp
        //           group grp by new { grp.ArticleId, grp.Status } into grp1
        //           select new Stock()
        //           {
        //               ArticleId = grp1.Key.ArticleId,
        //               Status = grp1.Key.Status,
        //               Quantity = grp1.Sum(x => GetQuantity(x))
        //           };                                                
        //}

        public async Task SaveChanges()
        {
            _context.SaveChanges();

            foreach (int invalidate in _invalidates)
            {
                await _mediator.Send(new InvalidateCacheCommand(invalidate));
            }
            _invalidates.Clear();
        }

        //private double GetQuantity(JoinedInventory item)
        //{
        //    if(item.Quantity == 0)
        //    {
        //        return item.Quantity;
        //    }

        //    double quantity = 0;
        //    if (item.MovementType == MovementType.In 
        //        && item.MovementStatus == MovementStatus.Closed)
        //    {
        //        //IN item that are arrived
        //        quantity =  item.Quantity;
        //    }
        //    else if (item.MovementType == MovementType.Out 
        //        && (item.MovementStatus == MovementStatus.Closed || item.MovementStatus == MovementStatus.Open))
        //    {
        //        //OUT item that are arived or pending arrival
        //        quantity = -item.Quantity;
        //    }
        //    return quantity;
        //}
        //private class JoinedInventory
        //{
        //    public int ArticleId { get; set; }
        //    public int Status { get; set; }
        //    public double Quantity { get; set; }
        //    public MovementType MovementType { get; set; }
        //    public MovementStatus MovementStatus { get; set; }
        //}
    }
}
