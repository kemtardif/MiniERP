using MediatR;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Commands
{
    public class CreateCommand : IRequest<Result<POReadDTO>>
    {
        public Guid TransactionID { get; }
        public POCreateDTO PurchaseOrder { get; }

        public CreateCommand(POCreateDTO dto)
        {
            PurchaseOrder = dto;
            TransactionID = Guid.NewGuid();
        }
    }
}
