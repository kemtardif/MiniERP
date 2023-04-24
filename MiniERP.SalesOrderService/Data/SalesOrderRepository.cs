using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using MiniERP.SalesOrderService.DTOs;
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
            return _context.SalesOrders.Include(x => x.Details);
        }

        public SalesOrder? GetSalesOrderById(int id)
        {
            return _context.SalesOrders.Include(x => x.Details).FirstOrDefault(x => x.Id == id);
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
            _context.SaveChanges();
        }

        public SalesOrder UpdateSalesOrder(SalesOrder item, JsonPatchDocument<UpdateSalesOrder> json)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (json is null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var dto = _mapper.Map<UpdateSalesOrder>(item);

            json.ApplyTo(dto);

            _mapper.Map(dto, item);

            return item;
        }
    }
}
