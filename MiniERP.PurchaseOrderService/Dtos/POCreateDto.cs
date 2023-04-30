using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.DTOs
{
    public class POCreateDTO
    {
        public int SupplierID { get; set; }
        public int Responsible { get; set; }
        public string Note { get; set; } = string.Empty;
        public PurchaseOrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public double TotalAmount { get; set; }
        public IEnumerable<PODetailCreateDTO> Details { get; set; } = new List<PODetailCreateDTO>();
    }
}
