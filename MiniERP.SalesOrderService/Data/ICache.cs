using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Data
{
    public interface ICache
    {
        StockModel? GetCachedStockModel(int id);
    }
}
