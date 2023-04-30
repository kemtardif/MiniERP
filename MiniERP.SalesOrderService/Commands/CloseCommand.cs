using MediatR;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Commands
{
    public class CloseCommand : IRequest<Result<SOReadDTO>>
    {
        public int Id { get; }
        public CloseCommand(int id)
        {
            Id = id;
        }
    }
}
