using MiniERP.PurchaseOrderService.Grpc.Protos;

namespace MiniERP.PurchaseOrderService.Grpc
{
    public class GrpcDataClient : IDataClient
    {
        private readonly GrpcInventoryService.GrpcInventoryServiceClient _grpcClient;

        public GrpcDataClient(GrpcInventoryService.GrpcInventoryServiceClient grpcClient)
        {
            _grpcClient = grpcClient ?? throw new ArgumentNullException(nameof(grpcClient));
        }

        public StockResponse GetAvailableStock(int articleId)
        {
            StockRequest request = new() { ArticleId = articleId };

            StockResponse response = _grpcClient.GetInventory(request);

            return response;
        }
    }
}
