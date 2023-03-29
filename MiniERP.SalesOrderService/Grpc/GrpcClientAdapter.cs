using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Grpc
{
    public class GrpcClientAdapter : IGrpcClientAdapter
    {
        private readonly GrpcInventory.GrpcInventoryClient _grpcClient;

        public GrpcClientAdapter(GrpcInventory.GrpcInventoryClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public StockResponse GetStockByArticleId(StockRequest request)
        {
            return _grpcClient.GetStockByArticleId(request);
        }
    }
}
