using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Grpc
{
    public interface IGrpcClientAdapter
    {
        StockResponse GetStockByArticleId(StockRequest request);

    }
}
