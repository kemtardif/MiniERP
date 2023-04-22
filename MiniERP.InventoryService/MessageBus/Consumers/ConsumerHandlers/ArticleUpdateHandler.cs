using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;
using System.Text.Json;

namespace MiniERP.InventoryService.MessageBus.Consumers.ConsumerHandlers
{
    public class ArticleUpdateHandler : IConsumerHandler
    {
        private readonly ILogger<ArticleUpdateHandler> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public ArticleUpdateHandler(ILogger<ArticleUpdateHandler> logger,
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
                ArticleUpdateMessage update = Deserialize(message) ?? throw new ArgumentException(nameof(update));
  
                UpdateArticleResponse(update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--->RabbitMQ Exception : {Instance} : {name} : {date}",
                                nameof(ArticleUpdateHandler),
                                nameof(Handle),
                                DateTime.UtcNow);
            }
        }

        private void UpdateArticleResponse(ArticleUpdateMessage dto)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                InventoryItemUpdate update = _mapper.Map<InventoryItemUpdate>(dto);
                   
                repo.Update(dto.Id, update);

                repo.SaveChanges();

                _logger.LogInformation("---> RabbitMQ : Stock updated from article : {id} : {date}",
                                               dto.Id,
                                               DateTime.UtcNow);
            }
        }

        private ArticleUpdateMessage? Deserialize(string message)
        {
            return JsonSerializer.Deserialize<ArticleUpdateMessage>(message);
        }
    }
}
