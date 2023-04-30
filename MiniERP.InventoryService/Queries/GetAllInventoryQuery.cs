using MediatR;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Queries
{
    public class GetAllInventoryQuery : IRequest<Result<IEnumerable<InventoryReadDTO>>>
    {
    }
}
