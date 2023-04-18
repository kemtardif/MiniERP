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

        public CancelStockResponse CancelStockMovement(CancelStockRequest request)
        {
            return _grpcClient.CancelStockMovement(request);
        }

        public CloseStockResponse CloseStockMovement(CloseStockRequest request)
        {
            return _grpcClient.CloseStockMovement(request);
        }

        public GetStockResponse GetStock(GetStockRequest request)
        {
            return _grpcClient.GetStock(request);
        }

        public OpenStockResponse OpenStockMovement(OpenStockRequest request)
        {
            return _grpcClient.OpenStockMovement(request);
        }
    }
}
