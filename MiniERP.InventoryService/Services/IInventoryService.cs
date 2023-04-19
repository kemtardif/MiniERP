using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Services
{
    public interface IInventoryService
    {
        Result<IEnumerable<InventoryItemReadDto>> GetAllInventories();
        Result<InventoryItemReadDto> GetInventoryByArticleId(int articleId);
    }
}
