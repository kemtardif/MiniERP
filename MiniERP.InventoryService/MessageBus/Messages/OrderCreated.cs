﻿namespace MiniERP.InventoryService.MessageBus.Messages
{
    public class OrderCreated : MessageBase
    {
        public int OrderId { get; set; }
        public int OrderType { get; set; }
        public Guid TransactionId { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
    public class OrderItem
    {
        public int ArticleId { get; set; }
        public double Quantity { get; set; }

    }
}
