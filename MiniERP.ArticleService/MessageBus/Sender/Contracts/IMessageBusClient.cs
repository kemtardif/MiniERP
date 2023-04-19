namespace MiniERP.ArticleService.MessageBus.Sender.Contracts
{
    public interface IMessageBusClient
    {
        void PublishMessage(string routingKey, string message);
    }

}
