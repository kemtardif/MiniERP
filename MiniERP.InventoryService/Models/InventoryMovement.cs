namespace MiniERP.InventoryService.Models
{
    public class InventoryMovement
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public InventoryItem InventoryItem { get; set; } = new();
        public MovementType MovementType { get; set; }
        public MovementStatus MovementStatus { get; set; }
        public double Quantity { get; set; }
        public int RelatedOrderId { get; set; }
        public RelatedOrderType RelatedOrderType { get; set; }

    }
    public enum MovementType
    {
        In = 1, 
        Out = 2
    }
    public enum MovementStatus
    {
        Open = 1,
        Closed = 2,
        Cancelled = 3
    }
    public enum RelatedOrderType
    {
        SO = 1,
        PO = 2
    }
}
