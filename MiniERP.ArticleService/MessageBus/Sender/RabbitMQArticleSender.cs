using AutoMapper;
using MiniERP.ArticleService.MessageBus.Sender.Contracts;
using MiniERP.ArticleService.MessageBus.Messages;
using MiniERP.ArticleService.Models;
using System.Text.Json;

namespace MiniERP.ArticleService.MessageBus.Sender
{
    public class RabbitMQArticleSender : IMessageBusSender<Article>
    {
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBus;

        private const string CreateRoutingKey = "article.create";
        private const string DeleteRoutingKey = "article.delete";
        private const string UpdateRoutingKey = "article.update";

        public RabbitMQArticleSender(
                                     IMapper mapper,
                                     IMessageBusClient messageBus)
        {
            _mapper = mapper;
            _messageBus = messageBus;
        }
        public void RequestForPublish(RequestType type, Article article)
        {
            switch (type)
            {
                case RequestType.Created:
                    var created = _mapper.Map<ArticleCreateMessage>(article);
                    string createdMsg = Serialize(created);
                    _messageBus.PublishMessage(CreateRoutingKey, createdMsg);
                    break;
                case RequestType.Deleted:
                    var deleted = _mapper.Map<ArticleDeleteMessage>(article);
                    string deletedMsg = Serialize(deleted);
                    _messageBus.PublishMessage(DeleteRoutingKey, deletedMsg);
                    break;
                case RequestType.Updated:
                    var updated = _mapper.Map<ArticleCreateMessage>(article);
                    string updatedMsg = Serialize(updated);
                    _messageBus.PublishMessage(UpdateRoutingKey, updatedMsg);
                    break;
                default:
                    throw new ArgumentNullException(nameof(type));
            }
        }
        private string Serialize<T>(T message)
        {
            return JsonSerializer.Serialize(message);
        }
    }
}
