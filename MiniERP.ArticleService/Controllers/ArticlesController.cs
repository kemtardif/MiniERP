using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Exceptions;
using MiniERP.ArticleService.Models;
using System.Net;

namespace MiniERP.ArticleService.Controllers;

[Authorize(Roles = "ApplicationHTTPRequestArtSrv")]
[ApiController]
[Route("api/art-srv/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly ILogger<ArticlesController> _logger;
    private readonly IArticleRepository _repository;
    private readonly IMapper _mapper;

    public ArticlesController(ILogger<ArticlesController> logger, IArticleRepository repository, IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<ArticleReadDto> GetAllArticles()
    {
        IEnumerable<Article> articles = _repository.GetAllArticles();
        return Ok(_mapper.Map<IEnumerable<ArticleReadDto>>(articles));
    }

    [HttpGet("{id}", Name = nameof(GetArticleById))]
    public ActionResult<ArticleReadDto> GetArticleById(int id)
    {
        Article? article = _repository.GetArticleById(id);
        if(article is null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<ArticleReadDto>(article));
    }
    [HttpPost]
    public ActionResult<ArticleReadDto> CreateArticle(ArticleWriteDto writeDto)
    {
        Article article = _mapper.Map<Article>(writeDto);
        if(!_repository.HasValidUnits(article.BaseUnitId))
        {
            return new UnprocessableEntityObjectResult(nameof(Unit));
        }
        article.CreatedAt = article.UpdatedAt = DateTime.UtcNow;

        _repository.AddArticle(article);
        try
        {
            _repository.SaveChanges();
        }
        catch (SaveChangesException ex)
        {
            _logger.LogError(ex.Message);
            return Problem(ex.Message);
        }
        ArticleReadDto readDto = _mapper.Map<ArticleReadDto>(article);
        return CreatedAtRoute(nameof(GetArticleById), new { id = readDto.Id }, readDto);
    }
    [HttpDelete("{id}")]
    public ActionResult<ArticleReadDto> RemoveArticle(int id)
    {
        Article? article = _repository.GetArticleById(id);
        if(article is  null)
        {
            return NotFound();
        }

        _repository.RemoveArticle(article);
        article.UpdatedAt = DateTime.UtcNow;

        try
        {
            _repository.SaveChanges();
        }
        catch(SaveChangesException ex)
        {
            _logger.LogError(ex.Message);
            return Problem(ex.Message);
        }

        return NoContent();
    }
    [HttpPatch("{id}")]
    public ActionResult<ArticleReadDto> UpdateArticle(int id, JsonPatchDocument<ArticleWriteDto> patchDoc )
    {
        Article? article = _repository.GetArticleById(id);
        if (article is null)
        {
            return NotFound();
            
        }

        try
        {
            var articleToWrite = _mapper.Map<ArticleWriteDto>(article);
            patchDoc.ApplyTo(articleToWrite);

            if(!TryValidateModel(articleToWrite))
            {
                return UnprocessableEntity();
            }
            if(!_repository.HasValidUnits(articleToWrite.BaseUnitId))
            {
                return UnprocessableEntity(nameof(articleToWrite.BaseUnitId));
            }
            _mapper.Map(articleToWrite, article);
            article.UpdatedAt = DateTime.UtcNow;
            _repository.SaveChanges();       
        }
        catch(JsonPatchException jsonex)
        {
            _logger.LogError(jsonex.Message);
            return UnprocessableEntity(jsonex.FailedOperation.path);
        }
        catch (SaveChangesException ex)
        {
            _logger.LogError(ex.Message);
            return Problem(ex.Message);
        }
        catch(ArgumentNullException nullEx)
        {
            _logger.LogError(nullEx.Message);
            return Problem(nullEx.Message);
        }
        ArticleReadDto readDto = _mapper.Map<ArticleReadDto>(article);
        return CreatedAtRoute(nameof(GetArticleById), new { id = readDto.Id }, readDto);
    }
}
