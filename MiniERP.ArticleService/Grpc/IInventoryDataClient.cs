using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Grpc
{
    public interface IInventoryDataClient
    {
        void InventoryItemsCreated(IEnumerable<Article> inventoryItems);
        void InventoryItemsUpdated(IEnumerable<Article> inventoryItems);
    }
}
