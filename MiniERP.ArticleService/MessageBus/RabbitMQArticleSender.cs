using AutoMapper;
using MiniERP.ArticleService.Dtos;
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
                    RequestForCreated(article);
                    break;
                case RequestType.Deleted:
                    RequestForDeleted(article);
                    break;
                default:
                    throw new ArgumentNullException(nameof(type));
            }
        }
     
        private void RequestForCreated(Article article)
        {
            if (article.IsInventory)
            {
                InventoryPublishDto publish = _mapper.Map<InventoryPublishDto>(article);
                publish.EventName = "Article_created";

                _messageBus.PublishNewArticle(publish, "inventory");
            }
        }

        private void RequestForDeleted(Article article)
        {
            if(article.IsInventory)
            {
                InventoryPublishDto publish = _mapper.Map<InventoryPublishDto>(article);
                publish.EventName = "Article_Deleted";

                _messageBus.PublishNewArticle(publish, "inventory");
            }
        }
    }
}
