using System.ComponentModel.DataAnnotations;

namespace MiniERP.ArticleService.Dtos
{
    public class UnitWriteDto
    {
        public string Name { get; set; } = string.Empty;
        public string UnitCode { get; set; } = string.Empty;
    }
}
