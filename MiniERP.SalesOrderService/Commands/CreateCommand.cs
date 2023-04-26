using MediatR;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Commands
{
    public class CreateCommand : IRequest<Result<SOReadDTO>>
    {
        public Guid TransactionId { get; set; } = Guid.NewGuid();
        public SOCreateDTO SalesOrder { get; set; } = new();
    }
}
