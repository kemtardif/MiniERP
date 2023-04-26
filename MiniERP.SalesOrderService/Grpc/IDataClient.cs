using Grpc.Core;
using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Grpc
{
    public interface IDataClient
    {
        StockResponse GetAvailableStock(int articleId);

    }
}
