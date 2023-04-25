using AutoMapper;
using MediatR;
using MiniERP.PurchaseOrderService.Commands;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Handlers
{
    public class CreateHandler : IRequestHandler<CreateCommand, Result<POReadDTO>>
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public CreateHandler(IRepository repository,
                             IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public Task<Result<POReadDTO>> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            var purchaseOrder = _mapper.Map<PurchaseOrder>(request.PurchaseOrder);

            _repository.AddPO(purchaseOrder);

            _repository.SaveChanges();

            var dto = _mapper.Map<POReadDTO>(purchaseOrder);

            return Task.FromResult(Result<POReadDTO>.Success(dto));
        }
    }
}
