namespace MiniERP.SalesOrderService.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public double Quantity { get; set; }
    }
    public class InventoryItemCache
    {
        public int ArticleId { get; set; }
        public int Status { get; set; }
        public double Quantity { get; set; }
    }
}
