namespace MiniERP.InventoryService.Dtos
{
    public class StockReadDto
    {
        public int ProductId { get; set; }
        public int Status { get; set; }
        public bool Discontinued { get; set; }
        public double Quantity { get; set; }
        public double MaxQuantity { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
    }
}
