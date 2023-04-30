using MediatR;
using MiniERP.PurchaseOrderService.Data;

namespace MiniERP.PurchaseOrderService.MessageBus.Handlers
{
    public abstract class HandlerBase<T> : IRequestHandler<T> where T : IRequest
    {
        private const string HandledLogFormat = "---> RabbitMQ : Message Handled : {msg} ";
        private const string ExceptionLogFormat = "---> RabbitMQ : Message Exception : {msg}";
        protected const string NotFoundLogFrmat = "ID={0} not found";

        protected ILogger<T> _logger;
        protected readonly IRepository _repository;

        public HandlerBase(ILogger<T> logger,
                           IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public virtual async Task Handle(T request, CancellationToken cancellationToken)
        {
            try
            {
                await ProtectedHandle(request);

                _logger.LogInformation(HandledLogFormat, typeof(T).Name);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ExceptionLogFormat, typeof(T).Name);
            }
        }

        protected abstract Task ProtectedHandle(T request);
    }
}
