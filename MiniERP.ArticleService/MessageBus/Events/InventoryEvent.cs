using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.MessageBus.Events
{
    public class InventoryEvent : GenericEvent
    {
        public int Id { get; set; }
        public double MaxQuantity { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
        public ArticleStatus Status { get; set; }
    }
}
