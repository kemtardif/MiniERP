using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Exceptions;
using MiniERP.PurchaseOrderService.Models;
using MiniERP.PurchaseOrderService.Services;


namespace MiniERP.PurchaseOrderService.Controllers
{
    [ApiController]
    [Authorize(Roles = "ApplicationHTTPRequestPoSrv")]
    [Route("api/po-srv/[controller]")]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly ILogger<PurchaseOrdersController> _logger;
        private readonly IPOService _service;

        public PurchaseOrdersController(ILogger<PurchaseOrdersController> logger,
                                        IPOService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public ActionResult GetAllPurchaseOrders()
        {            
            try
            {
                Result<IEnumerable<POReadDto>> result = _service.GetAllPurchaseOrders();

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException($"An error occured while getting all Purchase Orders", ex);
            }
        }

        [HttpGet("{id}", Name = nameof(GetPOByOId))]
        public ActionResult GetPOByOId(int id)
        {
            try
            {
                Result<POReadDto> result = _service.GetPOById(id);

                if(!result.IsSuccess)
                {
                    return NotFound(result.Errors);
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException($"An error occured while getting Purchase Order : ID={id}", ex);
            }
        }

        [HttpPost]
        public ActionResult CreatePurchaseOrder(POCreateDTO dto)
        {
            try
            {
                Result<POReadDto> result = _service.CreatePurchaseOrder(dto);

                if(!result.IsSuccess)
                {
                    return UnprocessableEntity(result.Errors);
                }

                return CreatedAtRoute(nameof(GetPOByOId), new { id = result.Value.Id}, result.Value);
            } 
            catch(Exception ex)
            {
                throw new HttpFriendlyException($"An error occured while creating Purchase Order", ex);
            }
        }
    }
}