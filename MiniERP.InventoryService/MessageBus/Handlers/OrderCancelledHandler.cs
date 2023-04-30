using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class OrderCancelledHandler : HandlerBase<OrderCancelled>
    {
        public OrderCancelledHandler(ILogger<OrderCancelled> logger, IInventoryRepository repository)
                                : base(logger, repository) { }

        protected override async Task ProtectedHandle(OrderCancelled request)
        {
            _repository.CancelMovementByOrder((RelatedOrderType)request.Type, request.Id);

            await _repository.SaveChanges();
        }
    }
}
