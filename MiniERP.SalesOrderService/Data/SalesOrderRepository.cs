using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using MiniERP.SalesOrderService.Dtos;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Data
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SalesOrderRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddSalesOrder(SalesOrder salesOrder)
        {
            if(salesOrder is null)
            {
                throw new ArgumentNullException(nameof(salesOrder));
            }
            _context.SalesOrders.Add(salesOrder);
        }

        public IEnumerable<SalesOrder> GetAllSalesOrders()
        {
            return _context.SalesOrders;
        }

        public SalesOrder? GetSalesOrderById(int id)
        {
            return _context.SalesOrders.FirstOrDefault(x => x.Id == id);
        }

        public void RemoveSalesOrder(SalesOrder salesOrder)
        {
            if(salesOrder is null)
            {
                throw new ArgumentNullException(nameof(salesOrder));
            }

            salesOrder.SetAsClosed();

        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public SalesOrder UpdateArticle(SalesOrder item, JsonPatchDocument<SalesOrderUpdateDto> json)
        {
            throw new NotImplementedException();
        }
    }
}
