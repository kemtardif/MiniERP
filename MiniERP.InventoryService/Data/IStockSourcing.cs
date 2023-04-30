using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public interface IStockSourcing
    {
        
        Stock? GetMinForecastById(int id);
        Stock? GetMaxForecastById(int id);
        IEnumerable<Stock> GetStockToAutoOrder();
    }
}
