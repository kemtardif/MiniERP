using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Services
{
    public interface IInventoryService
    {
        Result<IEnumerable<InventoryReadDTO>> GetAllInventories();
        Result<InventoryReadDTO> GetInventoryByArticleId(int articleId);
    }
}
