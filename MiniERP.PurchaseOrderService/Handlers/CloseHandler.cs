using AutoMapper;
using MediatR;
using MiniERP.PurchaseOrderService.Commands;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.MessageBus.Messages;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Handlers
{
    public class CloseHandler : HandlerBase, IRequestHandler<CloseCommand, Result<POReadDTO>>
    {
        public CloseHandler(IRepository repository, IMapper mapper) : base(repository, mapper) 
        {
        }
        public Task<Result<POReadDTO>> Handle(CloseCommand request, CancellationToken cancellationToken)
        {
            var po = _repository.GetPOById(request.Id);

            if(po is null)
            {
                return Task.FromResult(Result<POReadDTO>.Failure(GetNotFoundResult(request.Id)));
            }

            if(po.Status == PurchaseOrderStatus.Closed)
            {
                return Task.FromResult(Result<POReadDTO>.Failure(GetClosedResult(request.Id)));
            }

            po.Status = PurchaseOrderStatus.Closed;

            _repository.UpdatePO(po);

            _repository.SaveChanges();

            var dto = _mapper.Map<POReadDTO>(po);

            return Task.FromResult(Result<POReadDTO>.Success(dto, new OrderClosed(request.Id)));
        }

        private IDictionary<string, string[]> GetClosedResult(int id)
        {
            return new Dictionary<string, string[]>
            {
                [$"[{id}]"] = new string[] { $"Purchase Order is already closed" }
            };
        }
    }
}
