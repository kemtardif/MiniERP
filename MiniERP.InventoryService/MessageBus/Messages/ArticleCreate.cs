namespace MiniERP.InventoryService.MessageBus.Messages
{
    public class ArticleCreate : MessageBase
    {
        public int Id { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
        public int Status { get; set; }
    }
}
