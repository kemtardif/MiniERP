namespace MiniERP.PurchaseOrderService.DTOs
{
    public class PODetailCreateDto
    {
        public int LineNo { get; set; }
        public int Productd { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
