using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class OrderClosedHandler : HandlerBase<OrderClosed>
    {
        public OrderClosedHandler(ILogger<OrderClosed> logger, IInventoryRepository repository)
                            : base(logger, repository){ }
        protected override async Task ProtectedHandle(OrderClosed request)
        {
            _repository.CloseMovementByOrder((RelatedOrderType)request.Type, request.Id);

            await _repository.SaveChanges();
        }
    }
}
