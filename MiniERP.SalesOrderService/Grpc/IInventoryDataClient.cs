using MiniERP.SalesOrderService.Dtos;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Grpc
{
    public interface IInventoryDataClient
    {
        Stock? GetStockByArticleId(int articleId);
    }
}
