using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MiniERP.ArticleService.Commands;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.Exceptions;
using MiniERP.ArticleService.Models;
using MiniERP.ArticleService.Queries;

namespace MiniERP.ArticleService.Controllers;

[Authorize(Roles = "ApplicationHTTPRequestArtSrv")]
[ApiController]
[Route("api/art-srv/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly ILogger<ArticlesController> _logger;
    private readonly IMediator _mediator;

    public ArticlesController(ILogger<ArticlesController> logger,
                              IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ReadDTO>> GetAllArticles()
    {
        try
        {
            Result<IEnumerable<ReadDTO>> result = await _mediator.Send(new GetAllQuery());

            return Ok(result.Value);
        } 
        catch(Exception ex)
        {
            _logger.LogError("{id} : An error occured while getting all articles : {error} : {date}",
                                HttpContext.TraceIdentifier,
                                ex.Message,
                                DateTime.UtcNow);
            throw new HttpFriendlyException("An error occured while getting all articles", ex);
        }
    }

    [HttpGet("{id}", Name = nameof(GetArticleById))]
    public async Task<ActionResult<ReadDTO>> GetArticleById(int id)
    {
        try
        {
            Result<ReadDTO> result = await _mediator.Send(new GetByIdQuery(id));

            if (!result.IsSuccess)
            {
                return NotFound();
            }

            return Ok(result.Value);
        } 
        catch(Exception ex)
        {
            throw new HttpFriendlyException($"An error occured while getting article : ID = {id}", ex);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ReadDTO>> CreateArticle(CreateDTO writeDto)
    {
        try
        {
            Result<ReadDTO> result = await _mediator.Send(new CreateCommand(writeDto));

            if (!result.IsSuccess)
            {
                return UnprocessableEntity(result.Errors);
            }

            return CreatedAtRoute(nameof(GetArticleById), new { id = result.Value.Id }, result.Value);
        } 
        catch(Exception ex)
        {
            throw new HttpFriendlyException($"An error occured while creating article", ex);
        }       
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ReadDTO>> RemoveArticle(int id)
    {
        try
        {
            Result result = await _mediator.Send(new DeleteCommand(id));

            if (!result.IsSuccess)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch(Exception ex)
        {
            throw new HttpFriendlyException($"An error occured while removing article : {id}", ex);
        }
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<ReadDTO>?> UpdateArticle(int id, JsonPatchDocument<UpdateDTO> json )
    {
        try
        {
            Result<ReadDTO> result = await _mediator.Send(new UpdateCommand(id, json));

            if (!result.IsSuccess)
            {
                if (result.Errors.TryGetValue(nameof(Article), out string[]? value)
                    && value is not null)
                {
                    return NotFound(value);
                }
                else
                {
                    return UnprocessableEntity(result.Errors);
                }
            }

            return Ok(result.Value);
        } 
        catch(Exception ex)
        {
            throw new HttpFriendlyException($"An error occured while updating article : {id}", ex);
        }       
    }
}
