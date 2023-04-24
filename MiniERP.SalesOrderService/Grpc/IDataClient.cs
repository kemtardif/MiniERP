using Grpc.Core;
using MediatR;
using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Grpc
{
    public interface IDataClient
    {
        StockResponse GetAvailableStock(int articleId);

        AsyncDuplexStreamingCall<StockRequest, StockResponse> GetInventoryStream(CancellationToken token);
        IAsyncEnumerable<StockResponse> GetAvailableStockStream(IAsyncEnumerable<int> articleIds);
    }
}
