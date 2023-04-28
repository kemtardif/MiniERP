namespace MiniERP.PurchaseOrderService.DTOs
{
    public class PODetailCreateDTO
    {
        public int LineNo { get; set; }
        public int Productd { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
