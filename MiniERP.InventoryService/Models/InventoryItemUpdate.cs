namespace MiniERP.InventoryService.Models
{
    public class InventoryItemUpdate
    {
        public int Status { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
    }
}
