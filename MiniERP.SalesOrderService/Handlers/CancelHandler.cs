using AutoMapper;
using MiniERP.SalesOrderService.Commands;
using MiniERP.SalesOrderService.Data;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.MessageBus.Messages;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Handlers
{
    public class CancelHandler : HandlerBase<CancelCommand, Result<SOReadDTO>>
    {
        public CancelHandler(IRepository repository, IMapper mapper) : base(repository, mapper){ }
        public override Task<Result<SOReadDTO>> Handle(CancelCommand request, CancellationToken cancellationToken)
        {
            var po = _repository.GetSOById(request.Id);

            if (po is null)
            {
                return Task.FromResult(Result<SOReadDTO>.Failure(GetNotFoundResult(request.Id)));
            }

            if (po.Status != SalesOrderStatus.Open)
            {
                return Task.FromResult(Result<SOReadDTO>.Failure(GetALreadyCancelled(request.Id)));
            }

            po.Status = SalesOrderStatus.Cancelled;

            _repository.Update(po);

            _repository.SaveChanges();

            var dto = _mapper.Map<SOReadDTO>(po);

            return Task.FromResult(Result<SOReadDTO>.Success(dto, new OrderCancelled(request.Id)));
        }

        private IDictionary<string, string[]> GetALreadyCancelled(int id)
        {
            return new Dictionary<string, string[]>
            {
                [$"[{id}]"] = new string[] { $"SalesOrder Order already cancelled" }
            };
        }
    }
}
