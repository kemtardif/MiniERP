namespace MiniERP.SalesOrderService.Models
{
    public class Inventory
    {
        public List<InventoryItem> Items { get; set; } = new();

        public Inventory(List<InventoryItem> items)
        {
            Items = items;  
        }
    }

    public class InventoryItem
    {
        public int Id { get; set; }
        public double Quantity { get; set; }
    }
}
