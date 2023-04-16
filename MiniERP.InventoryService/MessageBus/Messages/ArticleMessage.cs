namespace MiniERP.InventoryService.MessageBus.Messages
{
    public class ArticleMessage : MessageBase
    {
        public int Id { get; set; }
        public double MaxQuantity { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
        public int Status { get; set; }
    }
}
