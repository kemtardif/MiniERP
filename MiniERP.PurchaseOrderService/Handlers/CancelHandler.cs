using AutoMapper;
using MediatR;
using MiniERP.PurchaseOrderService.Commands;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.MessageBus.Messages;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Handlers
{
    public class CancelHandler : HandlerBase, IRequestHandler<CancelCommand, Result<POReadDTO>>
    {
        public CancelHandler(IRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
        public Task<Result<POReadDTO>> Handle(CancelCommand request, CancellationToken cancellationToken)
        {
            var po = _repository.GetPOById(request.Id);

            if (po is null)
            {
                return Task.FromResult(Result<POReadDTO>.Failure(GetNotFoundResult(request.Id)));
            }

            if (po.Status == PurchaseOrderStatus.Cancelled)
            {
                return Task.FromResult(Result<POReadDTO>.Failure(GetALreadyCancelled(request.Id)));
            }

            po.Status = PurchaseOrderStatus.Cancelled;

            _repository.UpdatePO(po);

            _repository.SaveChanges();

            var dto = _mapper.Map<POReadDTO>(po);

            return Task.FromResult(Result<POReadDTO>.Success(dto, new OrderCancelled(request.Id)));
        }

        private IDictionary<string, string[]> GetALreadyCancelled(int id)
        {
            return new Dictionary<string, string[]>
            {
                [$"[{id}]"] = new string[] { $"Purchase Order is already cancelled" }
            };
        }
    }
}
