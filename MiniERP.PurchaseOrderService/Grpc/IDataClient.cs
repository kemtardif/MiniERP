using MiniERP.PurchaseOrderService.Grpc.Protos;

namespace MiniERP.PurchaseOrderService.Grpc
{
    public interface IDataClient
    {
        StockResponse GetAvailableStock(int articleId);

    }
}
