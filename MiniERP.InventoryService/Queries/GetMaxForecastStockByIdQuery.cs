using MediatR;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Queries
{
    public class GetMaxForecastStockByIdQuery : IRequest<Result<StockReadDTO>>
    {
        public int Id { get; }
        public GetMaxForecastStockByIdQuery(int id)
        {
            Id = id;
        }
    }
}
