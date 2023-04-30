using MediatR;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Queries
{
    public class GetAllQuery : IRequest<Result<IEnumerable<SOReadDTO>>>
    {
    }
}
