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
    private const string GetAllMessage = "An error occured while getting all articles";
    private const string GetByIdMessage = "An error occured while getting article : ID = {0}";
    private const string CreateMessage = "An error occured while creating article";
    private const string DeleteMessage = "An error occured while removing article : {0}";
    private const string UpdateMessage = "An error occured while updating article : {0}";

    private readonly IMediator _mediator;
    public ArticlesController(IMediator mediator)
    {
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
            throw new HttpFriendlyException(GetAllMessage, ex);
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
            throw new HttpFriendlyException(string.Format(GetByIdMessage, id), ex);
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
            throw new HttpFriendlyException(CreateMessage, ex);
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
            throw new HttpFriendlyException(string.Format(DeleteMessage, id), ex);
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
            throw new HttpFriendlyException(string.Format(UpdateMessage, id), ex);
        }       
    }
}
