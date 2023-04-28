using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Queries;

namespace MiniERP.InventoryService.Handlers
{
    public class GetAllInventoryHandler : HandlerBase<GetAllInventoryQuery, Result<IEnumerable<InventoryReadDTO>>>
    {
        public GetAllInventoryHandler(IRepository repository, IMapper mapper) : base(repository, mapper) { }
        public override Task<Result<IEnumerable<InventoryReadDTO>>> Handle(GetAllInventoryQuery request, CancellationToken cancellationToken)
        {
            var models = _repository.GetAllItems();

            var dtos = _mapper.Map<IEnumerable<InventoryReadDTO>>(models);

            return Task.FromResult(Result<IEnumerable<InventoryReadDTO>>.Success(dtos));
        }
    }
}
