using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public interface IInventoryRepository
    {
        //// InventoryItem
        IEnumerable<InventoryItem> GetAllItems();
        InventoryItem? GetInventoryById(int articleId);
        void AddItem(InventoryItem item);
        void CloseItem(int articleId);
        void Update(InventoryItem item);
        ////////////////////////////////////////////
        //// InventoryMovement
        void AddItem(InventoryMovement item);
        void CloseMovementByOrder(RelatedOrderType orderType, int orderId);
        void CancelMovementByOrder(RelatedOrderType orderType, int orderId);
        /////////////////////////////////////////
        Task SaveChanges();
    }
}
