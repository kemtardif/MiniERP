using MediatR;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Commands
{
    public class CreateCommand : IRequest<Result<POReadDto>>
    {
        public POCreateDTO PurchaseOrder { get; set; }

        public CreateCommand(POCreateDTO dto)
        {
            PurchaseOrder = dto;
        }
    }
}
