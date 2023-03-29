using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Dtos
{
    public class SalesOrderReadDto
    {
        public int Id { get; set; }
        public int CustID { get; set; }
        public string CustAddress { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public SalesOrderStatus Status { get; set; }
        public DateTime ConfirmDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public IEnumerable<SalesOrderDetailReadDto> Details { get; set; } = new List<SalesOrderDetailReadDto>();
    }
}
