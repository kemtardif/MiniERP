namespace MiniERP.InventoryService.MessageBus.Messages
{
    public class ArticleCreate : MessageBase
    {
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
        public int Status { get; set; }
    }
}
