namespace MiniERP.ArticleService.Models
{
    public record InventoryRecord
    {
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
        public ArticleStatus Status { get; set; } 
        public InventoryRecord(Article article)
        {
            AutoOrder= article.AutoOrder;
            AutoTreshold= article.AutoTreshold;
            AutoQuantity= article.AutoQuantity;
            Status = article.Status;
        }
    }
}
