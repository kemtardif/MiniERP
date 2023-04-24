using Grpc.Core;
using MiniERP.PurchaseOrderService.Grpc.Protos;

namespace MiniERP.PurchaseOrderService.Grpc
{
    public interface IDataClient
    {
        StockResponse GetAvailableStock(int articleId);

        AsyncDuplexStreamingCall<StockRequest, StockResponse> GetInventoryStream(CancellationToken token);
        IAsyncEnumerable<StockResponse> GetAvailableStockStream(IAsyncEnumerable<int> articleIds);
    }
}
