using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.MessageBus;
using MiniERP.ArticleService.Models;
using MiniERP.ArticleService.Services;
using System.Collections.Generic;
using System.Windows.Markup;

namespace MiniERP.ArticleService.Controllers;

[Authorize(Roles = "ApplicationHTTPRequestArtSrv")]
[ApiController]
[Route("api/art-srv/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly ILogger<ArticlesController> _logger;
    private readonly IArticleService _articleService;

    public ArticlesController(ILogger<ArticlesController> logger,
                            IArticleService articleService)
    {
        _logger = logger;
        _articleService = articleService;
    }

    [HttpGet]
    public ActionResult<ArticleReadDto> GetAllArticles()
    {
        try
        {
            Result<IEnumerable<ArticleReadDto>> result = _articleService.GetAllArticles();

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
    public ActionResult<ArticleReadDto> GetArticleById(int id)
    {
        try
        {
            Result<ArticleReadDto> result = _articleService.GetArticleById(id);

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
    public ActionResult<ArticleReadDto> CreateArticle(ArticleCreateDto writeDto)
    {
        try
        {
            Result<ArticleReadDto> result = _articleService.CreateArticle(writeDto);

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
    public ActionResult<ArticleReadDto> RemoveArticle(int id)
    {
        try
        {
            Result result = _articleService.RemoveArticleById(id);

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
    public ActionResult<ArticleReadDto> UpdateArticle(int id, JsonPatchDocument<ArticleUpdateDto> json )
    {
        try
        {
            Result<ArticleReadDto> result = _articleService.PatchArticle(id, json);

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
