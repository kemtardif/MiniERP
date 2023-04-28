using AutoMapper;
using MiniERP.SalesOrderService.Commands;
using MiniERP.SalesOrderService.Data;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.MessageBus.Messages;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Handlers
{
    public class CloseHandler : HandlerBase<CloseCommand, Result<SOReadDTO>>
    {
        public CloseHandler(IRepository repository, IMapper mapper) : base(repository, mapper){ }
        public override Task<Result<SOReadDTO>> Handle(CloseCommand request, CancellationToken cancellationToken)
        {
            var so = _repository.GetSOById(request.Id);

            if (so is null)
            {
                return Task.FromResult(Result<SOReadDTO>.Failure(GetNotFoundResult(request.Id)));
            }
            if(so.Status == SalesOrderStatus.Closed)
            {
                return Task.FromResult(Result<SOReadDTO>.Failure(GetClosedResult(request.Id)));
            }

            so.SetAsClosed();

            _repository.Update(so);

            _repository.SaveChanges();

            var dto = _mapper.Map<SOReadDTO>(so);

            return Task.FromResult(Result<SOReadDTO>.Success(dto, new OrderClosed(request.Id)));
        }
        private IDictionary<string, string[]> GetClosedResult(int id)
        {
            return new Dictionary<string, string[]>
            {
                [$"[{id}]"] = new string[] { $"Sales Order is already closed" }
            };
        }
    }
}
