using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Exceptions;
using MiniERP.ArticleService.MessageBus;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Controllers;

[Authorize(Roles = "ApplicationHTTPRequestArtSrv")]
[ApiController]
[Route("api/art-srv/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly ILogger<ArticlesController> _logger;
    private readonly IArticleRepository _repository;
    private readonly IMapper _mapper;
    private readonly IMessageBusSender<Article> _sender;

    public ArticlesController(ILogger<ArticlesController> logger, 
                                IArticleRepository repository, 
                                IMapper mapper,
                                IMessageBusSender<Article> sender)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
        _sender = sender;
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
        article.CreatedAt = article.UpdatedAt = DateTime.UtcNow;

        _repository.AddArticle(article);

        try
        {
            _repository.SaveChanges();
        }
        catch (SaveChangesException ex)
        {
            _logger.LogError("POST {methodName} : {exName} : {ex} : {date}", nameof(SaveChangesException), nameof(CreateArticle), ex.Message, DateTime.UtcNow);
            return Problem(ex.Message);
        }
        _logger.LogInformation("POST {methodName} : Article Created : {id} : {date}", nameof(CreateArticle), article.Id, DateTime.UtcNow);

        _sender.RequestForPublish(RequestType.Created, article);

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
            _logger.LogError("DELETE {methodName} SaveChangesException : {ex} -- {date}", nameof(RemoveArticle), ex.Message, DateTime.UtcNow);
            return Problem(ex.Message);
        }
        _logger.LogInformation("DELETE Article : {id} -- {date}", article.Id, DateTime.UtcNow);

        _sender.RequestForPublish(RequestType.Deleted, article);
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
            _logger.LogError("PATCH {methodName} JsonPatchException : {ex} -- {date}", nameof(UpdateArticle), jsonex.Message, DateTime.UtcNow);
            return UnprocessableEntity(jsonex.FailedOperation.path);
        }
        catch (SaveChangesException ex)
        {
            _logger.LogError("PATCH {methodName} SaveChangesException : {ex} -- {date}", nameof(UpdateArticle), ex.Message, DateTime.UtcNow);
            return Problem(ex.Message);
        }
        catch(ArgumentNullException nullEx)
        {
            _logger.LogError("PATCH {methodName} ArgumentNullException : {ex} -- {date}", nameof(UpdateArticle), nullEx.Message, DateTime.UtcNow);
            return Problem(nullEx.Message);
        }
        _logger.LogInformation("PATCH {methodName} Article Created : {id} -- {date}", nameof(UpdateArticle), article.Id, DateTime.UtcNow);

        ArticleReadDto readDto = _mapper.Map<ArticleReadDto>(article);
        return CreatedAtRoute(nameof(GetArticleById), new { id = readDto.Id }, readDto);
    }
}
