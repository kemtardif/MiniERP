
namespace MiniERP.ArticleService.Models
{
    public class Article
    {
        public int Id { get; set; }
        public long EAN { get; set;}
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ArticleType Type { get; set; } = ArticleType.SalePurchase;
        public ArticleStatus Status { get; set; } = ArticleStatus.Open;
        public double  BasePrice { get; set; }
        public int BaseUnitId { get; set; }
        public double MaxQuantity { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public void SetUpdatedAtToCurrentTime() => UpdatedAt = DateTime.UtcNow;
        public void SetCreatedAdToCurrentTime() => CreatedAt = DateTime.UtcNow;
        public void OpenArticle() => Status = ArticleStatus.Open;
        public void CloseArticle() => Status = ArticleStatus.Closed;

    }
    public enum ArticleStatus
    {
        Open = 1,
        Closed = 2
    }
    public enum ArticleType
    {
        Sale = 1,
        Purchase = 2,
        SalePurchase = 3
    }
}
