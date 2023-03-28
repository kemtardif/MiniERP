using MiniERP.ArticleService.Protos;

namespace MiniERP.ArticleService.Grpc
{
    public class GrpcClientAdapter : IGrpcClientAdapter
    {
        private readonly GrpcInventory.GrpcInventoryClient _grpcClient;

        public GrpcClientAdapter(GrpcInventory.GrpcInventoryClient grpcClient)
        {
            _grpcClient = grpcClient;
        }
        public GrpcInventoryResponse InventoryItemsCreated(InventoryItemRequest request, CancellationToken token)
        {
           return _grpcClient.InventoryItemsCreated(request, cancellationToken : token);
        }

        public GrpcInventoryResponse InventoryItemsUpdated(InventoryItemRequest request, CancellationToken token)
        {
            return _grpcClient.InventoryItemsUpdated(request, cancellationToken : token);
        }
    }
}
