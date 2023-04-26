using MediatR;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Commands
{
    public class CloseCommand : IRequest<Result<POReadDTO>>
    {
        public int Id { get; }

        public CloseCommand(int id)
        {
            Id = id;
        }
    }
}
