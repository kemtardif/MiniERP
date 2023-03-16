using CommonLib.Enums;

namespace MiniERP.ArticleService.Models
{
    public class Article
    {
        public int Id { get; set; }
        public long EAN { get; set;}
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsInventory { get; set; }
        public ArticleType Type { get; set; }
        public ArticleStatus Status { get; set; }
        public double  BasePrice { get; set; }
        public int BaseUnitId { get; set; }
        public double MaxQuantity { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public void SetUpdatedAtToNow() => UpdatedAt = DateTime.UtcNow;
        public void SetDatesToNow() => UpdatedAt = CreatedAt = DateTime.UtcNow;
        public void OpenStatus() => Status = ArticleStatus.Open;

    }
}
