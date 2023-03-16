using AutoMapper;
using CommonLib.Dtos;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.MessageBus
{
    public class RabbitMQArticleSender : IMessageBusSender<Article>
    {
        private readonly ILogger<RabbitMQArticleSender> _logger;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBus;

        public RabbitMQArticleSender(ILogger<RabbitMQArticleSender> logger, 
                                     IMapper mapper, 
                                     IMessageBusClient messageBus)
        {
            _logger = logger;
            _mapper = mapper;
            _messageBus = messageBus;
        }
        public void RequestForPublish(RequestType type, Article article)
        {
            switch(type)
            {
                case RequestType.Created:
                    InternalRequestForPublish(article, MessageBusEventType.ArticleCreated);
                    break;
                case RequestType.Deleted:
                    InternalRequestForPublish(article, MessageBusEventType.ArticleDeleted);
                    break;
                case RequestType.Updated:
                    InternalRequestForPublish(article, MessageBusEventType.ArticleUpdated);
                    break;
                default:
                    throw new ArgumentNullException(nameof(type));
            }
        }
     
        private void InternalRequestForPublish(Article article, string eventName)
        {
            ArticleEventDto publish = _mapper.Map<ArticleEventDto>(article);
            publish.EventName = eventName;

            _messageBus.PublishNewArticle(publish);
        }
    }
}
