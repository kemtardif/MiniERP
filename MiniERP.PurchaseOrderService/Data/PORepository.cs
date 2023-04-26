using Microsoft.EntityFrameworkCore;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Data
{
    public class PORepository : IRepository
    {
        private readonly AppDbContext _context;

        public PORepository(AppDbContext context)
        {
            _context = context;
        }
        public void AddPO(PurchaseOrder po)
        {
            if(po is null)
            {
                throw new ArgumentNullException(nameof(po));
            }
            _context.Add(po);
        }

        public IEnumerable<PurchaseOrder> GetAllPOs()
        {
            return _context.PurchaseOrders.Include(po => po.Details);
        }

        public PurchaseOrder? GetPOById(int id)
        {
            return _context.PurchaseOrders
                           .Include(po => po.Details)
                           .FirstOrDefault(po => po.Id == id);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void UpdatePO(PurchaseOrder po)
        {
           _context.Update(po);
        }
    }
}
