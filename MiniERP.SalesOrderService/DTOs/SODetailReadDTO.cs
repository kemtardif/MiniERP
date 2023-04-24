namespace MiniERP.SalesOrderService.DTOs
{
    public class SODetailReadDTO
    {
        public int ArticleId { get; set; }
        public int UnitId { get; set; }
        public int SupplierId { get; set; }
        public double Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Amount { get; set; }
    }
}
