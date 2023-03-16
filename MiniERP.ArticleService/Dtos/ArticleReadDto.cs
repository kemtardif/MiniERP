﻿using CommonLib.Enums;

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
        public bool IsInventory { get; set; }
        public double BasePrice { get; set; }
        public int BaseUnitId { get; set; }
        public double MaxQuantity { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
    }
}
