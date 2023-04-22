namespace MiniERP.ArticleService.MessageBus.Messages
{
    public class ArticleDeleteMessage : MessageBase 
    {
        public ArticleDeleteMessage()
        {
            Headers.Add("MessageType", "ArticleDelete");
        }
    }
}
