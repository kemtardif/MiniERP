using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;
using System.Text.Json;

namespace MiniERP.InventoryService.MessageBus.Consumers.ConsumerHandlers
{
    public class ArticleCreateHandler : IConsumerHandler
    {
        private readonly ILogger<ArticleCreateHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public ArticleCreateHandler(ILogger<ArticleCreateHandler> logger,
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
                ArticleCreateMessage create = Deserialize(message) ?? throw new ArgumentException(nameof(create));

                AddArticleResponse(create);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--->RabbitMQ Exception : {Instance} : {name} : {date}",
                                nameof(ArticleCreateHandler),
                                nameof(Handle),
                                DateTime.UtcNow);
            }
        }
        private void AddArticleResponse(ArticleMessage article)
        {
            using var scope = _scopeFactory.CreateScope();

            var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

            InventoryItem item = _mapper.Map<InventoryItem>(article);

            repo.AddItem(item);

            repo.SaveChanges();

            _logger.LogInformation("---> RabbitMQ : New Stock added: {id} : {date}",
                                           item.ArticleId,
                                           DateTime.UtcNow);
        }

        private ArticleCreateMessage? Deserialize(string data)
        {
            return JsonSerializer.Deserialize<ArticleCreateMessage>(data);
        }
    }
}
