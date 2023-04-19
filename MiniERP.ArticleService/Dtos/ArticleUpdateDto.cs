using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Dtos
{
    public class ArticleUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public long EAN { get; set; }
        public string Description { get; set; } = string.Empty;
        public ArticleType Type { get; set; }
        public double BasePrice { get; set; }
        public ArticleStatus Status { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
    }
}
