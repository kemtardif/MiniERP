using MediatR;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Commands
{
    public class CreateCommand : IRequest<Result<POReadDTO>>
    {
        public POCreateDTO PurchaseOrder { get; set; }

        public CreateCommand(POCreateDTO dto)
        {
            PurchaseOrder = dto;
        }
    }
}
