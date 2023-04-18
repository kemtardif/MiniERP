using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Grpc
{
    public interface IGrpcClientAdapter
    {
        OpenStockResponse OpenStockMovement(OpenStockRequest request);
        CloseStockResponse CloseStockMovement(CloseStockRequest request);
        CancelStockResponse CancelStockMovement(CancelStockRequest request);
        GetStockResponse GetStock(GetStockRequest request);

    }
}
