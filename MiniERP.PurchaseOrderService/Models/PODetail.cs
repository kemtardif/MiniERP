namespace MiniERP.PurchaseOrderService.Models
{
    public class PODetail
    {
        public int Id { get; set; }
        public int OrderNo { get; set; }
        public int LineNo { get; set; }
        public int Productd { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
