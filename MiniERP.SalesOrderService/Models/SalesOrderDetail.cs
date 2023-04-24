namespace MiniERP.SalesOrderService.Models
{
    public class SalesOrderDetail
    {
        public int Id { get; set; }
        public int SalesOrderId { get; set; }
        public int ArticleId { get; set; }
        public int UnitId { get; set; }
        public int SupplierId { get; set; }
        public double Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Amount { get; set; }     
    }
}
