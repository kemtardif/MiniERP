using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MiniERP.SalesOrderService.Dtos;
using MiniERP.SalesOrderService.Exceptions;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Services;

namespace MiniERP.SalesOrderService.Controllers
{
    [ApiController]
    [Authorize(Roles = "ApplicationHTTPRequestSoSrv")]
    [Route("api/so-srv/[controller]")]
    public class SalesOrdersController : ControllerBase
    {

        private readonly ILogger<SalesOrdersController> _logger;
        private readonly ISalesOrderService _service;

        public SalesOrdersController(ILogger<SalesOrdersController> logger,
                                     ISalesOrderService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<SalesOrderReadDto>> GetAllSalesOrder()
        {
            try
            {
                var result = _service.GetAllSalesOrders();

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException($"An error occured while getting all Sales Orders", ex);
            }
        }
        [HttpGet("{id}", Name = nameof(GetSalesOrderById))]
        public ActionResult<SalesOrderReadDto> GetSalesOrderById(int id)
        {
            try
            {
                var result = _service.GetSalesOrderById(id);

                if (!result.IsSuccess)
                {
                    return NotFound();
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException($"An error occured while getting Sales Order : ID={id}", ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<SalesOrderReadDto>> CreateSalesOrder(SalesOrderCreateDto dto)
        {
            try
            {
                var result = await _service.AddSalesOrder(dto);

                if (!result.IsSuccess)
                {
                    return UnprocessableEntity(result.Errors);
                }

                return CreatedAtRoute(nameof(GetSalesOrderById), new { id = result.Value.Id }, result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException("An error occured while creating Sales Order", ex);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteSalesOrder(int id)
        {
            try
            {
                Result result = _service.RemoveSalesOrderById(id);

                if(!result.IsSuccess)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException("An error occured while deleting Sales Order : ID={id}", ex);
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<SalesOrderReadDto>> PatchSalesOder(int id, JsonPatchDocument<SalesOrderUpdateDto> document)
        {
            try
            {
                Result<SalesOrderReadDto> result = await _service.UpdateSalesOrder(id, document);

                if (!result.IsSuccess)
                {
                    if(result.Errors.TryGetValue(nameof(SalesOrder), out string[]? errors)
                        && errors is not null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return UnprocessableEntity(result.Errors);
                    }
                }
                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException($"An error occured while updating Sales Order : ID={id}", ex);
            }
        }
    }
}