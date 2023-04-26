using AutoMapper;
using MediatR;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class ArticleCreateHandler : IRequestHandler<ArticleCreate>
    {
        private readonly ILogger<ArticleCreateHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository _repository;

        public ArticleCreateHandler(ILogger<ArticleCreateHandler> logger,
                                     IMapper mapper,
                                     IRepository repository)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
        }
        public Task Handle(ArticleCreate create, CancellationToken cancellationToken)
        {
            try
            {
                AddArticle(create);

                _logger.LogInformation("---> RabbitMQ : Message Handled : {handler} : {id} : {date}",
                                       nameof(ArticleCreateHandler),
                                       create.Id,
                                       DateTime.UtcNow);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--->RabbitMQ Exception : {Instance} : {name} : {date}",
                                nameof(ArticleCreateHandler),
                                nameof(Handle),
                                DateTime.UtcNow);
            }
            return Task.CompletedTask;
        }

        private void AddArticle(ArticleCreate article)
        {

            InventoryItem item = _mapper.Map<InventoryItem>(article);

            _repository.AddItem(item);

            _repository.SaveChanges();

            _logger.LogInformation("---> RabbitMQ : New Stock added: {id} : {date}",
                                           item.ArticleId,
                                           DateTime.UtcNow);
        }
    }
}
