using MediatR;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class OrderCreatedHandler : HandlerBase<OrderCreated>
    {
        public OrderCreatedHandler(ILogger<OrderCreated> logger, IRepository repository)
                                : base(logger, repository) { }

        protected override async Task ProtectedHandle(OrderCreated request)
        {
            int orderId = request.OrderId;
            RelatedOrderType orderType = (RelatedOrderType)request.OrderType;
            MovementType movementType = orderType == RelatedOrderType.SO ? MovementType.Out : MovementType.In;

            foreach (var item in request.Items)
            {

                InventoryItem article = _repository.GetInventoryByArticleId(item.ArticleId) ?? 
                    throw new ArgumentNullException(string.Format(NotFoundLogFrmat, item.ArticleId));

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

            await _repository.SaveChanges();
        }
    }
}
