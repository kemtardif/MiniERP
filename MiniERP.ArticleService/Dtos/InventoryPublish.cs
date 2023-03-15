﻿namespace MiniERP.ArticleService.Dtos
{
    public class InventoryPublishDto : GenericPublishDto
    {
        public int Id { get; set; }
        public double MaxQuantity { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }

    }
}
