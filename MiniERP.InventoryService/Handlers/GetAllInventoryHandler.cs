using AutoMapper;
using MediatR;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Queries;

namespace MiniERP.InventoryService.Handlers
{
    public class GetAllInventoryHandler : HandlerBase,  IRequestHandler<GetAllInventoryQuery, Result<IEnumerable<InventoryReadDTO>>>
    {
        private readonly IRepository _repository;

        public GetAllInventoryHandler(IRepository repository, IMapper mapper) : base(mapper) 
        {
            _repository = repository;
        }
        public Task<Result<IEnumerable<InventoryReadDTO>>> Handle(GetAllInventoryQuery request, CancellationToken cancellationToken)
        {
            var models = _repository.GetAllItems();

            var dtos = _mapper.Map<IEnumerable<InventoryReadDTO>>(models);

            return Task.FromResult(Result<IEnumerable<InventoryReadDTO>>.Success(dtos));
        }
    }
}
