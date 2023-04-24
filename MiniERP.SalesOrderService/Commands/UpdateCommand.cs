using MediatR;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Commands
{
    public class UpdateCommand : IRequest<Result<SOReadDTO>>
    {
    }
}
