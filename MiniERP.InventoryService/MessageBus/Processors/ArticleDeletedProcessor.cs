using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;
using System.Text.Json;

namespace MiniERP.InventoryService.MessageBus.Processors
{
    public class ArticleDeletedProcessor : ArticleProcessorBase
    {
        public override string ServiceType => MessageBusEventType.ArticleDeleted;
        public ArticleDeletedProcessor(ILogger<ArticleProcessorBase> logger,
                                       IServiceScopeFactory scopeFactory,
                                       IMapper mapper) : base(logger, scopeFactory, mapper) { }

        public override void ProcessMessage(string data)
        {
            try
            {
                ArticleMessage? dto = DeserializeToArticle(data);
                if (dto is null)
                {
                    return;
                }

                RemoveArticleResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--->RabbitMQ Exception : {Instance} : {name} : {date}",
                                nameof(ArticleDeletedProcessor),
                                nameof(ProcessMessage),
                                DateTime.UtcNow);
            }
        }

        private void RemoveArticleResponse(ArticleMessage dto)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                InventoryItem? item = repo.GetItemByArticleId(dto.Id, false, false);
                if (item is null)
                {
                    return;
                }

                repo.SetAsDiscontinued(item);

                repo.SaveChanges();

                _logger.LogInformation("---> RabbitMQ : Stock deleted for product: {id} : {date}",
                                               item.Id,
                                               DateTime.UtcNow);
            }
        }
    }
}
