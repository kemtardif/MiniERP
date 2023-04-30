namespace MiniERP.SalesOrderService.DTOs
{
    public class SOCreateDTO
    {
        public int CustID { get; set; }
        public string CustAddress { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime ConfirmDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public IEnumerable<SODetailCreateDTO> Details { get; set; } = new List<SODetailCreateDTO>();
    }
}
