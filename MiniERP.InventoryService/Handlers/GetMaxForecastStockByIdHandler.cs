using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Queries;

namespace MiniERP.InventoryService.Handlers
{
    public class GetMaxForecastStockByIdHandler : HandlerBase<GetMaxForecastStockByIdQuery, Result<StockReadDTO>>
    {
        private readonly IStockSourcing _source;
        public GetMaxForecastStockByIdHandler(IStockSourcing source, IMapper mapper) : base(mapper)
        {
            _source = source;
        }
        public override Task<Result<StockReadDTO>> Handle(GetMaxForecastStockByIdQuery request, CancellationToken cancellationToken)
        {
            Stock? stock = _source.GetMaxForecastById(request.Id);

            if (stock is null)
            {
                return Task.FromResult(Result<StockReadDTO>.Failure(GetNotFoundResult<Stock>(request.Id)));
            }

            var dto = _mapper.Map<StockReadDTO>(stock);

            return Task.FromResult(Result<StockReadDTO>.Success(dto));
        }
    }
}
