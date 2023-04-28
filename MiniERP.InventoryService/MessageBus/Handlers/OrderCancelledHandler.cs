using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class OrderCancelledHandler : HandlerBase<OrderCancelled>
    {
        public OrderCancelledHandler(ILogger<OrderCancelled> logger, IRepository repository)
                                : base(logger, repository) { }

        protected override async Task ProtectedHandle(OrderCancelled request)
        {
            var items = _repository.GetMovementsByOrder((RelatedOrderType)request.Type, request.Id)
                                   .ToList();

            foreach (var item in items)
            {
                item.MovementStatus = MovementStatus.Cancelled;
                _repository.Update(item);
            }

            await _repository.SaveChanges();
        }
    }
}
