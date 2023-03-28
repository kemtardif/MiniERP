using MiniERP.ArticleService.Protos;

namespace MiniERP.ArticleService.Grpc
{
    public interface IGrpcClientAdapter
    {
        GrpcInventoryResponse InventoryItemsCreated(InventoryItemRequest request, CancellationToken token);
        GrpcInventoryResponse InventoryItemsUpdated(InventoryItemRequest request, CancellationToken token);

    }
}
