namespace MiniERP.SalesOrderService.Exceptions
{
    public class HttpFriendlyException : Exception
    {
        public HttpFriendlyException(string message, Exception? exception) : base(message, exception) { }
    }
}
