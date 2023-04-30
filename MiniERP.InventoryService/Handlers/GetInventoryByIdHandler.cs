using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Queries;

namespace MiniERP.InventoryService.Handlers
{
    public class GetInventoryByIdHandler : HandlerBase<GetInventoryByIdQuery, Result<InventoryReadDTO>>
    {
        private readonly IInventoryRepository _repository;
        public GetInventoryByIdHandler(IInventoryRepository repository, IMapper mapper) : base(mapper) 
        {
            _repository = repository;
        }

        public override Task<Result<InventoryReadDTO>> Handle(GetInventoryByIdQuery request, CancellationToken cancellationToken)
        {
            var model = _repository.GetInventoryById(request.Id);

            if (model is null)
            {
                return Task.FromResult(Result<InventoryReadDTO>.Failure(GetNotFoundResult<InventoryItem>(request.Id)));
            }

            var dto = _mapper.Map<InventoryReadDTO>(model);

            return Task.FromResult(Result<InventoryReadDTO>.Success(dto));
        }
    }
}
