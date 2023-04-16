using MiniERP.PurchaseOrderService.Grpc.Protos;

namespace MiniERP.PurchaseOrderService.Grpc
{
    public interface IGrpcClientAdapter
    {
        StockResponse GetStockByArticleId(StockRequest request);
        StockChangedResponse StockChanged(StockChangedRequest request);
    }
}
