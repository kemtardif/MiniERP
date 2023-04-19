namespace MiniERP.InventoryService.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public int Status { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
        public double Quantity { get; set; }

        public void SetAsClosed()
        {
            Status = 2;
            AutoOrder = false;
        }
    }
}
