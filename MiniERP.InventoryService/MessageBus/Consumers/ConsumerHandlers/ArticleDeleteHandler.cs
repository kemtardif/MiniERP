using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;
using System.Text.Json;


namespace MiniERP.InventoryService.MessageBus.Consumers.ConsumerHandlers
{
    public class ArticleDeleteHandler : IConsumerHandler
    {
        private readonly ILogger<ArticleDeleteHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public ArticleDeleteHandler(ILogger<ArticleDeleteHandler> logger,
                                    IMapper mapper,
                                    IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _mapper = mapper;
            _scopeFactory = scopeFactory;
        }
        public void Handle(string message)
        {
            try
            {
                ArticleDeleteMessage delete = Deserialize(message) ?? throw new ArgumentException(nameof(delete));
     
                RemoveArticleResponse(delete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--->RabbitMQ Exception : {Instance} : {name} : {date}",
                                nameof(ArticleDeleteHandler),
                                nameof(Handle),
                                DateTime.UtcNow);
            }
        }

        private void RemoveArticleResponse(ArticleMessage dto)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                InventoryItem? item = repo.GetInventoryByArticleId(dto.Id);
                if (item is null)
                {
                    return;
                }

                repo.SetAsClosed(item.ArticleId);

                repo.SaveChanges();

                _logger.LogInformation("---> RabbitMQ : Stock deleted : {id} : {date}",
                                               item.Id,
                                               DateTime.UtcNow);
            }
        }

        private ArticleDeleteMessage? Deserialize(string message)
        {
            return JsonSerializer.Deserialize<ArticleDeleteMessage>(message);
        }
    }
}
