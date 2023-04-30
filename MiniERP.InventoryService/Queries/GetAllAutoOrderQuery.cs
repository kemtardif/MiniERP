using MediatR;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Queries
{
    public class GetAllAutoOrderQuery : IRequest<Result<IEnumerable<StockReadDTO>>>
    {
    }
}
