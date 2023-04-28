using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.DTOs
{
    public class POReadDTO
    {
        public int Id { get; set; }
        public int SupplierID { get; set; }
        public int Responsible { get; set; }
        public string Note { get; set; } = string.Empty;
        public PurchaseOrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public double TotalAmount { get; set; }
        public IEnumerable<PODetailReadDTO> Details { get; set; } = new List<PODetailReadDTO>();
    }
}
