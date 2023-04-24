using MediatR;
using MiniERP.SalesOrderService.Commands;
using MiniERP.SalesOrderService.Data;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Handlers
{
    public class DeleteHandler : IRequestHandler<DeleteCommand, Result<int>>
    {
        private readonly ISalesOrderRepository _repository;
        public DeleteHandler(ISalesOrderRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<Result<int>> Handle(DeleteCommand request, CancellationToken cancellationToken)
        {
            SalesOrder? so = _repository.GetSalesOrderById(request.Id);

            if (so is null)
            {
                return Task.FromResult(Result<int>.Failure(GetNotFoundResult(request.Id)));
            }

            _repository.RemoveSalesOrder(so);

            _repository.SaveChanges();

            return Task.FromResult(Result<int>.Success(request.Id));
        }

        private IDictionary<string, string[]> GetNotFoundResult(int articleId)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(SalesOrder)] = new string[] { $"Sales Order not found : ID = {articleId}" }
            };
        }
    }
}
