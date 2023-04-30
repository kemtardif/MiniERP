using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.DTOs
{
    public class UpdateDTO
    {
        public string Name { get; set; } = string.Empty;
        public long EAN { get; set; }
        public string Description { get; set; } = string.Empty;
        public ArticleType Type { get; set; }
        public double BasePrice { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }

        public static UpdateDTO CreateValidDTO()
        {
            return new UpdateDTO()
            {
                Name = "ValidName",
                EAN = 9999,
                Description = "ValidDescription",
                Type = ArticleType.Sale,
                BasePrice = 1.00,
                AutoOrder = false,
                AutoTreshold = 1.00,
                AutoQuantity = 0.00
            };
        }
    }
}
