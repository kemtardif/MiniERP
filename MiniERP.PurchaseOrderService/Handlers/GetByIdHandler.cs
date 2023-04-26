using AutoMapper;
using MediatR;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;
using MiniERP.PurchaseOrderService.Queries;

namespace MiniERP.PurchaseOrderService.Handlers
{
    public class GetByIdHandler : HandlerBase, IRequestHandler<GetByIdQuery, Result<POReadDTO>>
    {

        public GetByIdHandler(IRepository repository, IMapper mapper) : base(repository, mapper)
        {

        }
        public Task<Result<POReadDTO>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var model = _repository.GetPOById(request.Id);

            if(model is null)
            {
                Task.FromResult(Result<POReadDTO>.Failure(GetNotFoundResult(request.Id)));
            }

            var dto = _mapper.Map<POReadDTO>(model);

            return Task.FromResult(Result<POReadDTO>.Success(dto));
        }     
    }
}
