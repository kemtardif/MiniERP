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
public class InventoriesController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    public InventoriesController( IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<InventoryItemReadDto>> GetAllInventories()
    {
        try
        {
            Result<IEnumerable<InventoryItemReadDto>> result = _inventoryService.GetAllInventories();

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            throw new HttpFriendlyException($"An error occured while getting all stocks", ex);
        }
    }

    [HttpGet("{articleId}")]
    public ActionResult<InventoryItemReadDto> GetInventoryByArticleId(int articleId)
    {
        try
        {
            Result<InventoryItemReadDto> result = _inventoryService.GetInventoryByArticleId(articleId);

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
