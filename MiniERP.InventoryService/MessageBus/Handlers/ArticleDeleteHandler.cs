using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class ArticleDeleteHandler : IRequestHandler<ArticleDelete>
    {
        private readonly ILogger<ArticleDeleteHandler> _logger;
        private readonly IInventoryRepository _repository;
        public ArticleDeleteHandler(ILogger<ArticleDeleteHandler> logger,
                                     IInventoryRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public Task Handle(ArticleDelete request, CancellationToken cancellationToken)
        {
            try
            {
                RemoveArticle(request);

                _logger.LogInformation("---> RabbitMQ : Message Handled : {handler} : {id} : {}",
                                       nameof(ArticleDeleteHandler),
                                       request.Id,
                                       DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--->RabbitMQ Exception : {Instance} : {name} : {date}",
                                nameof(ArticleDeleteHandler),
                                nameof(Handle),
                                DateTime.UtcNow);
            }
            return Task.CompletedTask;
        }

        private void RemoveArticle(ArticleDelete request)
        {
            _repository.SetAsClosed(request.Id);

            _repository.SaveChanges();
        }
    }
}
