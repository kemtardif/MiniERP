﻿namespace MiniERP.InventoryService.Models.Views
{
    public class PendingInventoryView
    {
        public int ArticleId { get; set; }
        public int Status { get; set; }
        public double Quantity { get; set; }
    }
}
