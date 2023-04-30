using MediatR;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Queries
{
    public class GetAllQuery : IRequest<Result<IEnumerable<POReadDTO>>>
    {
    }
}
