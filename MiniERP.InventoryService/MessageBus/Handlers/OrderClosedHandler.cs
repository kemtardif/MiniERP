using MediatR;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class OrderClosedHandler : HandlerBase<OrderClosed>
    {
        public OrderClosedHandler(ILogger<OrderClosed> logger, IRepository repository)
                            : base(logger, repository){ }
        protected override async Task ProtectedHandle(OrderClosed request)
        {
            var items = _repository.GetMovementsByOrder((RelatedOrderType)request.Type, request.Id)
                                   .ToList();

            foreach (var item in items)
            {
                item.MovementStatus = MovementStatus.Closed;
                _repository.Update(item);
            }

            await _repository.SaveChanges();
        }
    }
}
