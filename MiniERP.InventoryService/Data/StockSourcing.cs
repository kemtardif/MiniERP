using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public class StockSourcing : IStockSourcing
    {
        private readonly AppDbContext _context;

        public StockSourcing(AppDbContext context)
        {
            _context = context;
        }
        public Stock? GetMinForecastById(int id)
        {
            Stock? stock = null;
            var item = _context.AvailableInventoryView
                .FirstOrDefault(x => x.ArticleId == id);

            if(item is not null)
            {
                stock = new Stock()
                {
                    ArticleId = item.ArticleId,
                    Status = item.Status,
                    Quantity = item.Quantity,
                };
            }
            return stock;
        }
        public Stock? GetMaxForecastById(int id)
        {
            Stock? stock = null;

            var pending = _context.PendingStockView.FirstOrDefault(x => x.ArticleId == id);

            if(pending is not null)
            {
                stock = new Stock()
                {
                    ArticleId = pending.ArticleId,
                    Status = pending.Status,
                    Quantity = pending.Quantity,
                };
            }
            return stock;
        }

        public IEnumerable<Stock> GetStockToAutoOrder()
        {
            var items = from it in _context.InventoryItems
                        join mv in _context.PendingStockView
                        on it.ArticleId equals mv.ArticleId into joined
                        where (it.AutoOrder == true && it.Status == 1 && it.AutoQuantity > 0)
                        from j in joined.DefaultIfEmpty()
                        where (j == null && it.AutoTreshold > 0) ||
                            (j != null && j.Quantity <= it.AutoTreshold)
                        select new Stock()
                        {
                            ArticleId = it.ArticleId,
                            Status = it.Status,
                            Quantity = it.AutoQuantity
                        };
            return items.AsEnumerable();
        }
        private class IdandStatus
        {
            public int ArticleId { get; set; }
            public int Status { get; set; }
        }
    }
}
