using MediatR;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Commands
{
    public class CreateCommand : IRequest<Result<SOReadDTO>>
    {
        public Guid TransactionId { get;  }
        public SOCreateDTO SalesOrder { get; } 
        public CreateCommand(SOCreateDTO salesOrder)
        {
            SalesOrder = salesOrder;
            TransactionId = Guid.NewGuid();
        }
    }
}
