using MediatR;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Commands
{
    public class CancelCommand : IRequest<Result<SOReadDTO>>
    {
        public int Id { get; }
        public CancelCommand(int id)
        {
            Id = id;
        }
    }
}
