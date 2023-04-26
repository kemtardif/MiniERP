using MediatR;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class OrderClosedHandler : IRequestHandler<OrderClosed>
    {
        private readonly ILogger<OrderClosedHandler> _logger;
        private readonly IRepository _repository;

        public OrderClosedHandler(ILogger<OrderClosedHandler> logger,
                                   IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public Task Handle(OrderClosed request, CancellationToken cancellationToken)
        {
            try
            {
                HandleOrderClosed(request);

                _logger.LogInformation("---> RabbitMQ : Message Handled : {handler} : {id}",
                                       nameof(OrderClosedHandler),
                                       request.Id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "---> RabbitMQ : Message Exception : {handler} : {id}",
                                        nameof(OrderClosedHandler),
                                       request.Id);
            }
            return Task.CompletedTask;
        }

        private void HandleOrderClosed(OrderClosed request)
        {
            var items = _repository.GetMovementsByOrder((RelatedOrderType)request.Type, request.Id)
                                   .ToList();

            foreach (var item in items)
            {
                item.MovementStatus = MovementStatus.Closed;
                _repository.Update(item);
            }

            _repository.SaveChanges();
        }
    }
}
