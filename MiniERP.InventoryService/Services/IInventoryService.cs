using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Services
{
    public interface IInventoryService
    {
        Result<IEnumerable<StockReadDto>> GetAllStocks();
        Task<Result<StockReadDto>> GetStockByArticleId(int articleId);
    }
}
