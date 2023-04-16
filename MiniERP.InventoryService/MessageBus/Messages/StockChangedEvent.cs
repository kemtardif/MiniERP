namespace MiniERP.InventoryService.MessageBus.Messages
{
    public class StockChangedEvent : GenericEvent
    {
        public IEnumerable<StockChange> Changes { get; set; } = new List<StockChange>();
    }
    public class StockChange
    {
        public int Id { get; set; }
        public double NewValue { get; set; }
    }
}
