using MediatR;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class OrderCancelledHandler : IRequestHandler<OrderCancelled>
    {
        private readonly ILogger<OrderCancelledHandler> _logger;
        private readonly IRepository _repository;

        public OrderCancelledHandler(ILogger<OrderCancelledHandler> logger,
                                   IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public Task Handle(OrderCancelled request, CancellationToken cancellationToken)
        {
            try
            {
                HandleOrderCancelled(request);

                _logger.LogInformation("---> RabbitMQ : Message Handled : {handler} : {id}",
                                       nameof(OrderCancelledHandler),
                                       request.Id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "---> RabbitMQ : Message Exception : {handler} : {id}",
                                        nameof(OrderCancelledHandler),
                                       request.Id);
            }
            return Task.CompletedTask;
        }

        private void HandleOrderCancelled(OrderCancelled request)
        {
            var items = _repository.GetMovementsByOrder((RelatedOrderType)request.Type, request.Id)
                                   .ToList();

            foreach (var item in items)
            {
                item.MovementStatus = MovementStatus.Cancelled;
                _repository.Update(item);
            }

            _repository.SaveChanges();
        }
    }
}
