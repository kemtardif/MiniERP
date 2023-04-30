using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.MessageBus.Messages
{
    public class ArticleCreateMessage : MessageBase
    {
        public int Id { get; set; }
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
        public ArticleStatus Status { get; set; }
        public ArticleCreateMessage()
        {
            Headers.Add("MessageType", "ArticleCreate");
        }
    }
}
