using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Dtos
{
    public class ArticleReadDto
    {
        public int Id { get; set; }
        public long EAN { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ArticleType Type { get; set; }
        public ArticleStatus Status { get; set; }
        public double BasePrice { get; set; }
        public int BaseUnitId { get; set; }
    }
}
