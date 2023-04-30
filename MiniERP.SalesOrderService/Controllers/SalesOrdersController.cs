using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.SalesOrderService.Commands;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Exceptions;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Queries;

namespace MiniERP.SalesOrderService.Controllers
{
    [ApiController]
    [Authorize(Roles = "ApplicationHTTPRequestSoSrv")]
    [Route("api/so-srv/[controller]")]
    public class SalesOrdersController : ControllerBase
    {
        private const string GetAllMessage = "An error occured while getting all Sales Orders";
        private const string GetByIdMessage = "An error occured while getting Sales Order : ID={0}";
        private const string CreateMessage = "An error occured while creating Sales Order";
        private const string CloseMessage = "An error occured while closing Sales Order";
        private const string CancelMessage = "An error occured while cancelling Sales Order";

        private readonly IMediator _mediator;
        public SalesOrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SOReadDTO>>> GetAllSalesOrder()
        {
            try
            {
                Result<IEnumerable<SOReadDTO>> result = await _mediator.Send(new GetAllQuery());

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException(GetAllMessage, ex);
            }
        }
        [HttpGet("{id}", Name = nameof(GetSalesOrderById))]
        public async Task<ActionResult<SOReadDTO>> GetSalesOrderById(int id)
        {
            try
            {
                Result<SOReadDTO> result = await _mediator.Send(new GetByIdQuery(id));

                if (!result.IsSuccess)
                {
                    return NotFound();
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException(string.Format(GetByIdMessage, id), ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<SOReadDTO>> CreateSalesOrder(SOCreateDTO dto)
        {
            try
            {
                Result<SOReadDTO> result = await _mediator.Send(new CreateCommand(dto) );

                if (!result.IsSuccess)
                {
                    return UnprocessableEntity(result.Errors);
                }

                return CreatedAtRoute(nameof(GetSalesOrderById), new { id = result.Value.Id }, result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException(CreateMessage, ex);
            }
        }

        [HttpPost("{id}/close")]
        public async Task<ActionResult> CloseSalesOrder(int id)
        {
            try
            {
                var result = await _mediator.Send(new CloseCommand(id));

                if (!result.IsSuccess)
                {
                    if (result.Errors.ContainsKey(nameof(SalesOrder)))
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
        public async Task<ActionResult> CancelSalesOrder(int id)
        {
            try
            {
                var result = await _mediator.Send(new CancelCommand(id));

                if (!result.IsSuccess)
                {
                    if (result.Errors.ContainsKey(nameof(SalesOrder)))
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