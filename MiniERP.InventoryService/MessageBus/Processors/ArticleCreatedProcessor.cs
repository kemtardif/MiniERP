using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Exceptions;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;
using System.Text.Json;

namespace MiniERP.InventoryService.MessageBus.Processors
{
    public class ArticleCreatedProcessor : ArticleProcessorBase
    {
        public override string ServiceType => MessageBusEventType.ArticleCreated;
        public ArticleCreatedProcessor(ILogger<ArticleProcessorBase> logger,
                                       IServiceScopeFactory scopeFactory,
                                       IMapper mapper) : base (logger, scopeFactory, mapper) { }
        public override void ProcessMessage(string data)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "--->RabbitMQ Exception : {Instance} : {name} : {date}",
                                nameof(ArticleCreatedProcessor),
                                nameof(ProcessMessage),
                                DateTime.UtcNow);
            }
        }
        private void AddArticleResponse(ArticleMessage article)
        {
            using var scope = _scopeFactory.CreateScope();

            var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

            InventoryItem item = _mapper.Map<InventoryItem>(article);

            repo.AddInventoryItem(item);

            repo.SaveChanges();

            _logger.LogInformation("---> RabbitMQ : New product saved has new Stock: {id} : {date}",
                                           item.Id,
                                           DateTime.UtcNow);
        }
    }
}
