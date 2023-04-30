using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Exceptions;
using MiniERP.InventoryService.Queries;
using Quartz;

namespace MiniERP.InventoryService.Controllers
{
    [ApiController]
    [Authorize(Roles = "ApplicationHTTPRequestInvSrv")]
    [Route("/api/inv-srv/[controller]")]
    public class StockQueriesController : ControllerBase
    {
        private const string GetAllAutoMessage = "An error occured while getting all auto-orders";
        private const string GetByIdMessage = "An error occured while getting stock : ID={0}";

        private readonly IMediator _mediator;
        public StockQueriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("minforecast/{articleId}")]
        public async Task<ActionResult<StockReadDTO>> GetMinForecastById(int articleId)
        {
            try
            {
                var result = await _mediator.Send(new GetMinForecastStockByIdQuery(articleId));

                if (!result.IsSuccess)
                {
                    return NotFound();
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException(string.Format(GetByIdMessage, articleId), ex);
            }
        }

        [HttpGet("maxforecast/{articleId}")]
        public async Task<ActionResult<StockReadDTO>> GetMaxForecastById(int articleId)
        {
            try
            {
                var result = await _mediator.Send(new GetMaxForecastStockByIdQuery(articleId));

                if (!result.IsSuccess)
                {
                    return NotFound();
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException(string.Format(GetByIdMessage, articleId), ex);
            }
        }

        [HttpGet("autoOrders")]
        public async Task<ActionResult<IEnumerable<StockReadDTO>>> GetAutoOrderStocks()
        {
            try
            {
                var result = await _mediator.Send(new GetAllAutoOrderQuery());

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                throw new HttpFriendlyException(GetAllAutoMessage, ex);
            }
        }
    }
}
