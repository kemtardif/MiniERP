using Microsoft.AspNetCore.Mvc;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Exceptions;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using MiniERP.InventoryService.Queries;

namespace MiniERP.InventoryService.Controllers;

[ApiController]
[Authorize(Roles = "ApplicationHTTPRequestInvSrv")]
[Route("/api/inv-srv/[controller]")]
public class InventoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryReadDTO>>> GetAllInventories()
    {
        try
        {
            var result = await _mediator.Send(new GetAllInventoryQuery());

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            throw new HttpFriendlyException($"An error occured while getting all stocks", ex);
        }
    }

    [HttpGet("{articleId}")]
    public async Task<ActionResult<InventoryReadDTO>> GetInventoryByArticleId(int articleId)
    {
        try
        {
            var result = await _mediator.Send(new GetInventoryByIdQuery(articleId));

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
