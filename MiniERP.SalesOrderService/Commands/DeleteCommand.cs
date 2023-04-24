using MediatR;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Commands
{
    public class DeleteCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }
}
