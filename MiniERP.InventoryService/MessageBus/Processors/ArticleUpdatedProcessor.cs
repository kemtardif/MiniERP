using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Processors
{
    public class ArticleUpdatedProcessor : ArticleProcessorBase
    {
        public override string ServiceType => MessageBusEventType.ArticleUpdated;
        public ArticleUpdatedProcessor(ILogger<ArticleProcessorBase> logger,
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

                UpdateArticleResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--->RabbitMQ Exception : {Instance} : {name} : {date}",
                                nameof(ArticleUpdatedProcessor),
                                nameof(ProcessMessage),
                                DateTime.UtcNow);
            }
        }

        private void UpdateArticleResponse(ArticleMessage dto)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                InventoryItem? item = repo.GetItemByArticleId(dto.Id, false, false);
                if (item is null)
                {
                    return;
                }

                _mapper.Map(dto, item);

                repo.SaveChanges();

                _logger.LogInformation("---> RabbitMQ : Stock updated from article : {id} : {date}",
                                               item.Id,
                                               DateTime.UtcNow);
            }
        }
    }
}
