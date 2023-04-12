using MiniERP.PurchaseOrderService.Dtos;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Services
{
    public interface IPOService
    {
        Result<IEnumerable<POReadDto>> GetAllPurchaseOrders();
        Result<POReadDto> GetPOById(int id);
        Result<POReadDto> CreatePurchaseOrder(POCreateDto createDto);
    }
}
