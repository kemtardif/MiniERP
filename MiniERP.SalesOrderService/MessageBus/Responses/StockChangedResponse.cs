namespace MiniERP.SalesOrderService.MessageBus.Responses
{
    public class StockChangedResponse : GenericResponse
    {
        public IEnumerable<StockChange> Changes { get; set; } = new List<StockChange>();
    }
    public class StockChange
    {
        public int Id { get; set; }
        public double NewValue { get; set; }
    }
}
