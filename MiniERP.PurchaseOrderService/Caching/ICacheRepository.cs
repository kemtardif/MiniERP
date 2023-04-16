using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Caching
{
    public interface ICacheRepository
    {
        Task<Stock?> GetStockByArticleId(int articleId);
        Task SetStock(Stock stock);
    }
}
