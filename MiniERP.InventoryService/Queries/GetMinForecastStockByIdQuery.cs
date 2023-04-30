using MediatR;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Queries
{
    public class GetMinForecastStockByIdQuery : IRequest<Result<StockReadDTO>>
    {
        public int Id { get; }
        public GetMinForecastStockByIdQuery(int id)
        {
            Id = id;
        }
    }
}
