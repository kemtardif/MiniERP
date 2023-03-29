namespace MiniERP.InventoryService.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public double Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public void SetUpdatedAtToCurrentTime() => UpdatedAt = DateTime.UtcNow;
        public void SetCreatedAdToCurrentTime() => CreatedAt = DateTime.UtcNow;
    }
}
