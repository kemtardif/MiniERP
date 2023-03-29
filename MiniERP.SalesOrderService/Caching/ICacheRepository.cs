

using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Caching
{
    public interface ICacheRepository
    {
        Task<Stock?> GetStockByArticleId(int articleId);
        Task SetStock(Stock stock);
    }
}
