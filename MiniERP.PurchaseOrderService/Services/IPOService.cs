using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Services
{
    public interface IPOService
    {
        Result<IEnumerable<POReadDto>> GetAllPurchaseOrders();
        Result<POReadDto> GetPOById(int id);
        Result<POReadDto> CreatePurchaseOrder(POCreateDTO createDto);
    }
}
