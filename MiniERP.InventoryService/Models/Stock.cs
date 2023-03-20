namespace MiniERP.InventoryService.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Status { get; set; }
        public bool Discontinued { get; set; }
        public double Quantity { get; set; }
        public double MaxQuantity { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}
        public void SetUpdatedAtToCurrentTime() => UpdatedAt = DateTime.UtcNow;
        public void SetCreatedAdToCurrentTime() => CreatedAt = DateTime.UtcNow;
        public void SetAsDiscontinued()
        {
            Discontinued = true;
            AutoOrder = false;
        }
        public void SetAsClosed()
        {
            Status = 2;
            AutoOrder = false;
        }
    }
 
}
