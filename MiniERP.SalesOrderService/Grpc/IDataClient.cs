using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Grpc
{
    public interface IDataClient
    {
        StockResponse GetAvailableStock(int articleId);
        IAsyncEnumerable<StockResponse> GetAvailableStockStream(IAsyncEnumerable<int> articleIds);
    }
}
