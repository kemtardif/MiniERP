using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Data
{
    public interface IRepository
    {
        IEnumerable<PurchaseOrder> GetAllPOs();
        PurchaseOrder? GetPOById(int id);
        void AddPO(PurchaseOrder po);
        void UpdatePO(PurchaseOrder po);   
        void SaveChanges();
    }
}
