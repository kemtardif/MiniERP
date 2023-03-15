using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Dtos
{
    public class InventoryPublishDto : ArticlePublishDto
    {       
        public double MaxQuantity { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }

    }
}
