namespace MiniERP.InventoryService.DTOs
{
    public class InventoryReadDTO
    {
        public int ArticleId { get; set; }
        public int Status { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
    }
}
