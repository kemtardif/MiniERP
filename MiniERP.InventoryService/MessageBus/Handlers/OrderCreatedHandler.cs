using MediatR;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class OrderCreatedHandler : IRequestHandler<OrderCreated>
    {
        private readonly ILogger<OrderCreatedHandler> _logger;
        private readonly IRepository _repository;

        public OrderCreatedHandler(ILogger<OrderCreatedHandler> logger,
                                   IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public Task Handle(OrderCreated request, CancellationToken cancellationToken)
        {
            try
            {
                HandleOrderCreated(request);

                _logger.LogInformation("---> RabbitMQ : Message Handled : {handler} : {id} : {type} : {guid}",
                                       nameof(OrderCreatedHandler),
                                       request.OrderId,
                                       Enum.GetName((RelatedOrderType)request.OrderType),
                                       request.TransactionId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "---> RabbitMQ : Message Exception : {handler} : {id} : {type} : {guid}",
                                        nameof(OrderCreatedHandler),
                                       request.OrderId,
                                       Enum.GetName((RelatedOrderType)request.OrderType),
                                       request.TransactionId);
            }
            return Task.CompletedTask;
        }

        private void HandleOrderCreated(OrderCreated request)
        {
            int orderId = request.OrderId;
            RelatedOrderType orderType = (RelatedOrderType)request.OrderType;
            MovementType movementType = orderType == RelatedOrderType.SO ? MovementType.Out : MovementType.In;

            foreach (var item in request.Items)
            {

                InventoryItem article = _repository.GetInventoryByArticleId(item.ArticleId) ?? 
                    throw new ArgumentNullException($"Inventory ID={item.ArticleId} not found");

                InventoryMovement movement = new()
                {
                    ArticleId = article.ArticleId,
                    InventoryItem = article,
                    MovementType = movementType,
                    MovementStatus = MovementStatus.Open,
                    Quantity = item.Quantity,
                    RelatedOrderId = orderId,
                    RelatedOrderType = orderType

                };

                _repository.AddItem(movement);
            }

            _repository.SaveChanges();
        }
    }
}
