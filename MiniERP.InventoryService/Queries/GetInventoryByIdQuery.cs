using MediatR;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Queries
{
    public class GetInventoryByIdQuery : IRequest<Result<InventoryReadDTO>>
    {
        public int Id { get; }
        public GetInventoryByIdQuery(int id)
        {
            Id = id;
        }
    }
}
