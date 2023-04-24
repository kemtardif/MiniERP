using AutoMapper;
using MediatR;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;
using MiniERP.PurchaseOrderService.Queries;

namespace MiniERP.PurchaseOrderService.Handlers
{
    public class GetAllHandler : IRequestHandler<GetAllQuery, Result<IEnumerable<POReadDto>>>
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public GetAllHandler(IRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public Task<Result<IEnumerable<POReadDto>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
        {
            var models = _repository.GetAllPOs();

            var dtos = _mapper.Map<IEnumerable<POReadDto>>(models);

            return Task.FromResult(Result<IEnumerable<POReadDto>>.Success(dtos));
        }
    }
}
