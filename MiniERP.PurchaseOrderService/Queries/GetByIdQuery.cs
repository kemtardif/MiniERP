using MediatR;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Queries
{
    public class GetByIdQuery : IRequest<Result<POReadDTO>>
    {
        public int Id { get; }

        public GetByIdQuery(int id)
        {
            Id = id;
        }
    }
}
