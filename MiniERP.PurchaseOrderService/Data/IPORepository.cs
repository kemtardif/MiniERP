using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Data
{
    public interface IPORepository
    {
        IEnumerable<PurchaseOrder> GetAllPOs();
        PurchaseOrder? GetPOById(int id);
        void AddPO(PurchaseOrder po);
        //PurchaseOrder UpdatePO(PurchaseOrder item, JsonPatchDocument<SalesOrderUpdateDto> json);
        void SaveChanges();
    }
}
