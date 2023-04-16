using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Exceptions;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;
using System.Text.Json;

namespace MiniERP.InventoryService.MessageBus.Processors
{
    public class ArticleCreatedProcessor : IMessageProcessor
    {
        private readonly ILogger<ArticleCreatedProcessor> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;
        public string ServiceType => MessageBusEventType.ArticleCreated;

        public ArticleCreatedProcessor(ILogger<ArticleCreatedProcessor> logger,
                                       IServiceScopeFactory scopeFactory,
                                       IMapper mapper)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessMessage(string data)
        {
            try
            {
                ArticleMessage? article = DeserializeToArticle(data);
                if (article is null)
                {
                    return;
                }

                AddArticleResponse(article);

            }
            catch (JsonException ex)
            {
                _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                nameof(ProcessMessage),
                                nameof(JsonException),
                                ex.Message,
                                DateTime.UtcNow);
            }
            catch (SaveChangesException sCEx)
            {
                _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                nameof(ProcessMessage),
                                nameof(SaveChangesException),
                                sCEx.Message,
                                DateTime.UtcNow);
            }
        }
        private void AddArticleResponse(ArticleMessage article)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                InventoryItem item = _mapper.Map<InventoryItem>(article);
                item.Stock.Quantity = 150;
                item.MaxQuantity = 200;

                repo.AddInventoryItem(item);

                repo.SaveChanges();

                _logger.LogInformation("---> RabbitMQ : New product saved has new Stock: {id} : {date}",
                                               item.ProductId,
                                               DateTime.UtcNow);

            }
        }
        private ArticleMessage? DeserializeToArticle(string data)
        {
            ArticleMessage? article = JsonSerializer.Deserialize<ArticleMessage>(data);

            if (article is null)
            {
                _logger.LogInformation("---> RabbitMQ : Deserialized dto is null : {date}",
                                    DateTime.UtcNow);
            }
            return article;
        }
    }
}
