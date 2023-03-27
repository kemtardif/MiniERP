using Microsoft.AspNetCore.JsonPatch;
using MiniERP.SalesOrderService.Dtos;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Services
{
    public interface ISalesOrderService
    {
        Result<IEnumerable<SalesOrderReadDto>> GetAllSalesOrders();
        Result<SalesOrderReadDto> GetSalesOrderById(int id);
        Result<SalesOrderReadDto> AddSalesOrder(SalesOrderCreateDto salesOrder);
        Result RemoveSalesOrderById(int id);
        Result<SalesOrderReadDto> UpdateSalesOrder(int id, JsonPatchDocument<SalesOrderUpdateDto> json);
    }
}
