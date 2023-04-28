using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.PurchaseOrderService.Commands;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Exceptions;
using MiniERP.PurchaseOrderService.Models;
using MiniERP.PurchaseOrderService.Queries;

namespace MiniERP.PurchaseOrderService.Controllers
{
    [ApiController]
    [Authorize(Roles = "ApplicationHTTPRequestPoSrv")]
    [Route("api/po-srv/[controller]")]
    public class PurchaseOrdersController : ControllerBase
    {
        private const string GetAllMessage = "An error occured while getting all Purchase Orders";
        private const string GetByIdMessage = "An error occured while getting Purchase Order : ID={0}";
        private const string CreateMessage = "An error occured while creating Purchase Order";
        private const string CloseMessage = "An error occured while closing Purchase Order";
        private const string CancelMessage = "An error occured while cancelling Purchase Order";

        private readonly IMediator _mediator;
        public PurchaseOrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllPurchaseOrders()
        {
            try
            {
                var result = await _mediator.Send(new GetAllQuery());

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException(GetAllMessage, ex);
            }
        }

        [HttpGet("{id}", Name = nameof(GetPOByOId))]
        public async Task<ActionResult> GetPOByOId(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetByIdQuery(id));

                if (!result.IsSuccess)
                {
                    return NotFound(result.Errors);
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException(string.Format(GetByIdMessage, id), ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreatePurchaseOrder(POCreateDTO dto)
        {
            try
            {
                var result = await _mediator.Send(new CreateCommand(dto));

                if (!result.IsSuccess)
                {
                    return UnprocessableEntity(result.Errors);
                }

                return CreatedAtRoute(nameof(GetPOByOId), new { id = result.Value.Id }, result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException(CreateMessage, ex);
            }
        }

        [HttpPost("{id}/close")]
        public async Task<ActionResult> ClosePurchaseOrder(int id)
        {
            try
            {
                var result = await _mediator.Send(new CloseCommand(id));

                if (!result.IsSuccess)
                {
                    if(result.Errors.ContainsKey(nameof(PurchaseOrder)))
                    {
                        return NotFound();
                    }
                    return UnprocessableEntity(result.Errors);
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException(CloseMessage, ex);
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelPurchaseOrder(int id)
        {
            try
            {
                var result = await _mediator.Send(new CancelCommand(id));

                if (!result.IsSuccess)
                {
                    if (result.Errors.ContainsKey(nameof(PurchaseOrder)))
                    {
                        return NotFound();
                    }
                    return UnprocessableEntity(result.Errors);
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException(CancelMessage, ex);
            }
        }
    }
}