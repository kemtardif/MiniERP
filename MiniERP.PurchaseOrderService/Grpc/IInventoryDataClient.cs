using MiniERP.PurchaseOrderService.Grpc.Protos;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Grpc
{
    public interface IInventoryDataClient
    {
        Stock? GetStockByArticleId(int articleId);
        IEnumerable<Stock> StockChanged(IEnumerable<StockChangedModel> changed);
    }
}
