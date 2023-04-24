using MediatR;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Queries
{
    public class GetByIdQuery : IRequest<Result<POReadDto>>
    {
        public int Id { get; set; }

        public GetByIdQuery(int id)
        {
            Id = id;
        }
    }
}
