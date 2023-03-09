namespace MiniERP.ArticleService.Models
{
    public class Article
    {
        public int Id { get; set; }
        public long EAN { get; set;}
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ArticleType Type { get; set; }
        public ArticleStatus Status { get; set; }
        public double  BasePrice { get; set; }
        public int BaseUnitId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
    public enum ArticleStatus
    {
        Open = 1,
        Closed = 2,
        Parked = 3
    }
    public enum ArticleType
    {
        Sale = 1,
        Purchase = 2,
        SalePurchase = 3
    }
}
