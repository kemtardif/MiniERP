namespace MiniERP.SalesOrderService.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public int InventoryId { get; set; }
        public double Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
