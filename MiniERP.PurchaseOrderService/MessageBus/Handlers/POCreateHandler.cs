using MediatR;
using MiniERP.PurchaseOrderService.Commands;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.MessageBus.Messages;
using MiniERP.PurchaseOrderService.Models;
using System.Text.Json;

namespace MiniERP.PurchaseOrderService.MessageBus.Handlers
{
    public class POCreateHandler : HandlerBase<POCreate>
    {
        private readonly IMediator _mediator;

        public POCreateHandler(ILogger<POCreate> logger, 
                               IRepository repository,
                               IMediator mediator) : base(logger, repository)
        {
            _mediator = mediator;
        }
        protected override async Task ProtectedHandle(POCreate request)
        {

            POCreateDTO dto = CreatePOInternal(request);

            await _mediator.Send(new CreateCommand(dto));
        }


        //// Demo purpose.
        POCreateDTO CreatePOInternal(POCreate request)
        {
            return new POCreateDTO()
            {
                SupplierID = 1,
                Responsible = 2,
                Note = "Create Internal",
                Status = PurchaseOrderStatus.Open,
                OrderDate = DateTime.UtcNow,
                DeliveryDate = DateTime.UtcNow,
                TotalAmount = 0,
                Details = new List<PODetailCreateDTO>()
                {
                    new PODetailCreateDTO()
                    {
                        LineNo = 1,
                        Productd = request.Id,
                        Quantity = request.Quantity,
                        UnitPrice = 0,
                        Notes = "Detail Create Internal"
                    }
                }
            };
        }


    }
}
