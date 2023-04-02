using MiniERP.SalesOrderService.Dtos;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Grpc
{
    public interface IInventoryDataClient
    {
        Stock? GetStockByArticleId(int articleId);
        IEnumerable<Stock> StockChanged(IEnumerable<StockChangedModel> changed);
    }
}
