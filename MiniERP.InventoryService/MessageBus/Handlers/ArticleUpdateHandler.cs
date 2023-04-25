using AutoMapper;
using MediatR;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class ArticleUpdateHandler : IRequestHandler<ArticleUpdate>
    {
        private readonly ILogger<ArticleCreateHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IInventoryRepository _repository;
        public ArticleUpdateHandler(ILogger<ArticleCreateHandler> logger,
                                    IMapper mapper,
                                    IInventoryRepository repository)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
        }
        public Task Handle(ArticleUpdate request, CancellationToken cancellationToken)
        {
            try
            {
                Update(request);

                _logger.LogInformation("---> RabbitMQ : Message Handled : {handler} : {id} : {date}",
                                       nameof(ArticleUpdateHandler),
                                       request.Id,
                                       DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--->RabbitMQ Exception : {Instance} : {name} : {date}",
                                nameof(ArticleUpdateHandler),
                                nameof(Handle),
                                DateTime.UtcNow);
            }
            return Task.CompletedTask;
        }
        private void Update(ArticleUpdate request)
        {
            InventoryItem? item = _repository.GetInventoryByArticleId(request.Id);

            if(item is null)
            {
                _logger.LogWarning("--->RabbitMQ Exception : {Instance} : Could not find item : {id} : {date}",
                                nameof(ArticleUpdateHandler),
                                request.Id,
                                DateTime.UtcNow);
                return;
            }

            _mapper.Map(request, item);

            _repository.Update(item);

            _repository.SaveChanges();
        }

    }
}
