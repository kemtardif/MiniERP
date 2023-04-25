using AutoMapper;
using MediatR;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;
using MiniERP.PurchaseOrderService.Queries;

namespace MiniERP.PurchaseOrderService.Handlers
{
    public class GetByIdHandler : IRequestHandler<GetByIdQuery, Result<POReadDTO>>
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdHandler(IRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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

        private IDictionary<string, string[]> GetNotFoundResult(int id)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(PurchaseOrder)] = new string[] { $"Purchase Order not found : ID = {id}" }
            };
        }
    }
}
