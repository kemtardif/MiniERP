namespace MiniERP.ArticleService.MessageBus.Messages
{
    public class ArticleDeleteMessage : MessageBase 
    {
        public int Id { get; set; }
        public ArticleDeleteMessage()
        {
            Headers.Add("MessageType", "ArticleDelete");
        }
    }
}
