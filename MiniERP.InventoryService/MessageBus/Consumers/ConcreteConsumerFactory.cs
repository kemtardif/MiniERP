using AutoMapper;

using MiniERP.InventoryService.MessageBus.Consumers.ConsumerHandlers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MiniERP.InventoryService.MessageBus.Consumers
{
    public class ConcreteConsumerFactory : IConsumerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        private const string ArticleCreateRK = "article.create";
        private const string ArticleDeleteRK = "article.delete";
        private const string ArticleUpdateRK = "article.update";

        public ConcreteConsumerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public EventingBasicConsumer CreateConsumer(IModel model, string routingKey)
        {
            
            switch (routingKey)
            {
                case ArticleCreateRK:
                    return CreateArticleInstance(model);
                case ArticleDeleteRK:
                    return DeleteArticleInstance(model);
                case ArticleUpdateRK:
                    return UpdateArticleInstance(model);
                default:
                    throw new NotImplementedException();
            }
        }
        private EventingBasicConsumer CreateArticleInstance(IModel model)
        {
            ILogger<RabbitMQConsumer> loggerCOnsumer = _serviceProvider.GetRequiredService<ILogger<RabbitMQConsumer>>();
            ILogger<ArticleCreateHandler> loggerHandler = _serviceProvider.GetRequiredService<ILogger<ArticleCreateHandler>>();
            IMapper mapper = _serviceProvider.GetRequiredService<IMapper>();
            IServiceScopeFactory scopefactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            return new RabbitMQConsumer(loggerCOnsumer, model, new ArticleCreateHandler(loggerHandler, mapper, scopefactory));
        }

        private EventingBasicConsumer DeleteArticleInstance(IModel model)
        {
            ILogger<RabbitMQConsumer> loggerCOnsumer = _serviceProvider.GetRequiredService<ILogger<RabbitMQConsumer>>();
            ILogger<ArticleDeleteHandler> loggerHandler = _serviceProvider.GetRequiredService<ILogger<ArticleDeleteHandler>>();
            IMapper mapper = _serviceProvider.GetRequiredService<IMapper>();
            IServiceScopeFactory scopefactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            return new RabbitMQConsumer(loggerCOnsumer, model, new ArticleDeleteHandler(loggerHandler, mapper, scopefactory)); ;
        }

        private EventingBasicConsumer UpdateArticleInstance(IModel model)
        {
            ILogger<RabbitMQConsumer> loggerCOnsumer = _serviceProvider.GetRequiredService<ILogger<RabbitMQConsumer>>();
            ILogger<ArticleUpdateHandler> loggerHandler = _serviceProvider.GetRequiredService<ILogger<ArticleUpdateHandler>>();
            IMapper mapper = _serviceProvider.GetRequiredService<IMapper>();
            IServiceScopeFactory scopefactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            return new RabbitMQConsumer(loggerCOnsumer, model, new ArticleUpdateHandler(loggerHandler, mapper, scopefactory));
        }
    }
}
