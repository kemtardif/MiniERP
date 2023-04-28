using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Caching
{
    public interface ICacheService
    {
        AvailableInventoryView? GetById(int articleId);
        void Invalidate(int articleId);
    }
}
