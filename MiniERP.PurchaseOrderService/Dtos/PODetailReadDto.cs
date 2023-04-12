namespace MiniERP.PurchaseOrderService.Dtos
{
    public class PODetailReadDto
    {
        public int Id { get; set; }
        public int OrderNo { get; set; }
        public int LineNo { get; set; }
        public int Productd { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
