using MediatR;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Commands
{
    public class CreateCommand : IRequest<Result<SOReadDTO>>
    {
        public SOCreateDTO SalesOrder { get; set; } = new();
    }
}
