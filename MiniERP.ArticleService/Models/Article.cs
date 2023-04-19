
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
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
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
