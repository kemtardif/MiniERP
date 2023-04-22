using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.MessageBus.Messages
{
    public class ArticleUpdateMessage : MessageBase
    {
        public bool AutoOrder { get; set; }
        public double AutoTreshold { get; set; }
        public double AutoQuantity { get; set; }
        public ArticleStatus Status { get; set; }
        public ArticleUpdateMessage()
        {
            Headers.Add("MessageType", "ArticleUpdate");
        }
    }
}
