namespace MiniERP.SalesOrderService.Models
{
    public class SalesOrder
    {
        public int Id { get; set; }
        public int CustID { get; set; }
        public string CustAddress { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public SalesOrderStatus Status { get; set; }
        public DateTime ConfirmDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public void SetUpdatedAtToCurrentTime() => UpdatedAt = DateTime.UtcNow;
        public void SetCreatedAdToCurrentTime() => CreatedAt = DateTime.UtcNow;
        public void SetAsParked() => Status = SalesOrderStatus.Parked;
        public void SetAsClosed() => Status = SalesOrderStatus.Closed;
        public IEnumerable<SalesOrderDetail> Details { get; set; } = Array.Empty<SalesOrderDetail>();
    }

    public enum SalesOrderStatus
    {
        Open = 1,
        Parked = 2,
        Closed = 3

    }
}
