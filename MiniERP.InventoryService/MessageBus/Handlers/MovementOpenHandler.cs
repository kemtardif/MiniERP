using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;
using System.Text.Json;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class MovementOpenHandler : IRequestHandler<MovementOpen>
    {
        private readonly ILogger<MovementOpenHandler> _logger;
        private readonly IInventoryRepository _repository;
        public MovementOpenHandler(ILogger<MovementOpenHandler> logger,
                                   IInventoryRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public Task Handle(MovementOpen request, CancellationToken cancellationToken)
        {
            try
            {
                AddMovement(request);

                _logger.LogInformation("---> RabbitMQ : Message Handled : {handler} : {id} : {type} : {date}",
                                       nameof(MovementOpenHandler),
                                       request.RelatedOrderId,
                                       Enum.GetName((RelatedOrderType)request.RelatedOrderType),
                                       DateTime.UtcNow);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "---> RabbitMQ : Message Exception : {handler} : {id} : {type} : {date}",
                                    nameof(MovementOpenHandler),
                                    request.RelatedOrderId,
                                    Enum.GetName((RelatedOrderType)request.RelatedOrderType),
                                    DateTime.UtcNow);
            }
            return Task.CompletedTask;
        }

        private void AddMovement(MovementOpen request)
        {
            foreach(var item in request.MovementItems)
            {
                InventoryItem? article = _repository.GetInventoryByArticleId(item.ArticleId);

                if(article is null)
                {
                    throw new ArgumentNullException($"Inventory ID={item.ArticleId} not found");
                }

                InventoryMovement movement = new()
                {
                    ArticleId = article.ArticleId,
                    InventoryItem = article,
                    MovementType = (MovementType)item.MovementType,
                    MovementStatus = MovementStatus.Closed,
                    Quantity = item.Quantity,
                    RelatedOrderId = request.RelatedOrderId,
                    RelatedOrderType = (RelatedOrderType)request.RelatedOrderType

                };

                _repository.AddItem(movement);
            }

            _repository.SaveChanges();
        }
    }
}
