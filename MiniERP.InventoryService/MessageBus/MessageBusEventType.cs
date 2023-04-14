namespace MiniERP.InventoryService.MessageBus
{
    public static class MessageBusEventType
    {
        public const string StockChanged = "stock_changed";

        public const string ArticleCreated = "Article_created";
        public const string ArticleDeleted = "Article_Deleted";
        public const string ArticleUpdated = "Article_Updated";
    }
}
