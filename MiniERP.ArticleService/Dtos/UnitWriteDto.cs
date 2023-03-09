using System.ComponentModel.DataAnnotations;

namespace MiniERP.ArticleService.Dtos
{
    public class UnitWriteDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string UnitCode { get; set; } = string.Empty;
    }
}
