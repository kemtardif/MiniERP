using MiniERP.InventoryService.Models.Views;

namespace MiniERP.InventoryService.Caching
{
    public interface ICacheService
    {
        AvailableInventoryView? GetById(int articleId);
        void Invalidate(int articleId);
    }
}
