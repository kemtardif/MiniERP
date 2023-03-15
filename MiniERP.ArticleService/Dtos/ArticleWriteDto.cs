using MiniERP.ArticleService.Models;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace MiniERP.ArticleService.Dtos
{
    public class ArticleWriteDto
    {
        [Required]
        [StringLength(255, MinimumLength = 5)]   
        public string Name { get; set; } = string.Empty;
        [Required]
        public long EAN { get; set; }
        [StringLength(255)]
        public string Description { get; set; } = string.Empty;
        [Required]
        [EnumDataType(typeof(ArticleType))]
        public ArticleType Type { get; set; }
        [EnumDataType(typeof(ArticleStatus))]
        public ArticleStatus Status { get; set; }
        public bool IsInventory { get; set; }
        public double BasePrice { get; set; }
        [Required]
        public int BaseUnitId { get; set; }
        public double MaxQuantity { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
    }
}
