using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public interface ICache
    {
        AvailableInventoryView? GetById(int articleId);
        void Invalidate(int articleId);
    }
}
