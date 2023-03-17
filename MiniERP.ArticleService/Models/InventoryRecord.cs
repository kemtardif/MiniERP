namespace MiniERP.ArticleService.Models
{
    public record InventoryRecord
    {
        public double MaxQuantity { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
        public ArticleStatus Status { get; set; } = ArticleStatus.Open;
        public InventoryRecord(Article article)
        {
            MaxQuantity= article.MaxQuantity;
            AutoOrder= article.AutoOrder;
            AutoTreshold= article.AutoTreshold;
            AutoQuantity= article.AutoQuantity;
            Status = article.Status;
        }
    }
}
