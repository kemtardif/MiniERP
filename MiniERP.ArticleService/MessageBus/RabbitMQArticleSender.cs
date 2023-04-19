using AutoMapper;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.MessageBus.Messages;
using MiniERP.ArticleService.Models;
using System.Text.Json;

namespace MiniERP.ArticleService.MessageBus
{
    public class RabbitMQArticleSender : IMessageBusSender<Article>
    {
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBus;

        public RabbitMQArticleSender(
                                     IMapper mapper,
                                     IMessageBusClient messageBus)
        {
            _mapper = mapper;
            _messageBus = messageBus;
        }
        public void RequestForPublish(RequestType type, Article article)
        {
            switch(type)
            {
                case RequestType.Created:
                    var created = _mapper.Map<ArticleCreateMessage>(article);
                    string createdMsg = Serialize(created);
                    _messageBus.PublishMessage("article.create", createdMsg);
                    break;
                case RequestType.Deleted:
                    var deleted = _mapper.Map<ArticleDeleteMessage>(article);
                    string deletedMsg = Serialize(deleted);
                    _messageBus.PublishMessage("article.delete", deletedMsg);
                    break;
                case RequestType.Updated:
                    ArticleMessage updated = _mapper.Map<ArticleCreateMessage>(article);
                    var updatedMsg = Serialize(updated);
                    _messageBus.PublishMessage("article.update", updatedMsg);
                    break;
                default:
                    throw new ArgumentNullException(nameof(type));
            }
        }
        private string Serialize<T>(T message)
        {
            return JsonSerializer.Serialize<T>(message);
        }
    }
}
