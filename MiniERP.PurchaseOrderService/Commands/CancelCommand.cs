using MediatR;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Commands
{
    public class CancelCommand : IRequest<Result<POReadDTO>>
    {
        public int Id { get; }
        public CancelCommand(int id)
        {
            Id = id;
        }
    }
}
