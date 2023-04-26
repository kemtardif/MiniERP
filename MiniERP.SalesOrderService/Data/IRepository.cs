using Microsoft.AspNetCore.JsonPatch;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Data
{
    public interface IRepository
    {
        IEnumerable<SalesOrder> GetAllSalesOrders();
        SalesOrder? GetSOById(int id);
        void AddSalesOrder(SalesOrder salesOrder);
        void RemoveSalesOrder(SalesOrder salesOrder);
        SalesOrder UpdateSalesOrder(SalesOrder item, JsonPatchDocument<UpdateSalesOrder> json);
        void Update(SalesOrder so);
        void SaveChanges();

    }
}
