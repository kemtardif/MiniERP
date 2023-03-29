using Microsoft.AspNetCore.Mvc;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Exceptions;
using Microsoft.AspNetCore.Authorization;
using MiniERP.InventoryService.Services;

namespace MiniERP.InventoryService.Controllers;

[ApiController]
[Authorize(Roles = "ApplicationHTTPRequestInvSrv")]
[Route("/api/inv-srv/[controller]")]
public class StocksController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    public StocksController( IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<StockReadDto>> GetAllStocks()
    {
        try
        {
            Result<IEnumerable<StockReadDto>> result = _inventoryService.GetAllStocks();

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            throw new HttpFriendlyException($"An error occured while getting all stocks", ex);
        }
    }

    [HttpGet("{articleId}")]
    public ActionResult<StockReadDto> GetStockByArticleId(int articleId)
    {
        try
        {
            Result<StockReadDto> result = _inventoryService.GetStockByArticleId(articleId);

            if(!result.IsSuccess)
            {
                return NotFound();
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            throw new HttpFriendlyException($"An error occured while getting stock : ArticlID={articleId}", ex);
        }
    }
}
