using System.ComponentModel.DataAnnotations.Schema;

namespace MiniERP.InventoryService.Models
{
    public class AvailableInventoryView
    {
        public int ArticleId { get; set; }
        public int Status { get; set; }
        public double Quantity { get; set; }
    }
}
