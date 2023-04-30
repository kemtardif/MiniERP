using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Queries;

namespace MiniERP.InventoryService.Handlers
{
    public class GetAllAutoOrderHandler : HandlerBase<GetAllAutoOrderQuery, Result<IEnumerable<StockReadDTO>>>
    {
        private readonly IStockSourcing _source;
        public GetAllAutoOrderHandler(IStockSourcing source, IMapper mapper) : base(mapper)
        {
            _source = source;
        }

        public override Task<Result<IEnumerable<StockReadDTO>>> Handle(GetAllAutoOrderQuery request, CancellationToken cancellationToken)
        {
            var items = _source.GetStockToAutoOrder();

            var dtos = _mapper.Map<IEnumerable<StockReadDTO>>(items);

            return Task.FromResult(Result<IEnumerable<StockReadDTO>>.Success(dtos));
        }
    }
}
