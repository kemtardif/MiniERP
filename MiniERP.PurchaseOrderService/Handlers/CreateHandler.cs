using AutoMapper;
using MediatR;
using MiniERP.PurchaseOrderService.Commands;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.MessageBus.Messages;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Handlers
{
    public class CreateHandler : HandlerBase<CreateCommand, Result<POReadDTO>>
    {
        public CreateHandler(IRepository repository, IMapper mapper) : base(repository, mapper){}
        public override Task<Result<POReadDTO>> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            var purchaseOrder = _mapper.Map<PurchaseOrder>(request.PurchaseOrder);

            _repository.AddPO(purchaseOrder);

            _repository.SaveChanges();

            var dto = _mapper.Map<POReadDTO>(purchaseOrder);

            var message = _mapper.Map<OrderCreated>(dto);
            message.TransactionId = request.TransactionID;

            return Task.FromResult(Result<POReadDTO>.Success(dto, message));
        }
    }
}
