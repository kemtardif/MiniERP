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
        public void SetAsParked() => Status = SalesOrderStatus.Cancelled;
        public void SetAsClosed() => Status = SalesOrderStatus.Closed;
        public IEnumerable<SalesOrderDetail> Details { get; set; } = new List<SalesOrderDetail>();
    }

    public enum SalesOrderStatus
    {
        Open = 1,
        Cancelled = 2,
        Closed = 3
    }
}
