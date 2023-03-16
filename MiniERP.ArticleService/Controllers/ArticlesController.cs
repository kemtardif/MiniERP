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
    private readonly IValidator<Article> _validator;

    public ArticlesController(ILogger<ArticlesController> logger, 
                                IArticleRepository repository, 
                                IMapper mapper,
                                IMessageBusSender<Article> sender,
                                IValidator<Article> validator)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
        _sender = sender;
        _validator = validator;
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
    public ActionResult<ArticleReadDto> CreateArticle(ArticleCreateDto writeDto)
    {
        Article article = _mapper.Map<Article>(writeDto);
        article.OpenStatus();

        if(!Validate(article))
        {
            return UnprocessableEntity(ModelState);
        }

        _repository.AddArticle(article);

        article.SetDatesToNow();
        _repository.SaveChanges();

        _logger.LogInformation("POST : Article Created : {id} : {date}", article.Id, DateTime.UtcNow);

        _sender.RequestForPublish(RequestType.Created, article);

        ArticleReadDto readDto = _mapper.Map<ArticleReadDto>(article);
        return CreatedAtRoute(nameof(GetArticleById), new { id = readDto.Id }, readDto);
    }
    [HttpDelete("{id}")]
    public ActionResult<ArticleReadDto> RemoveArticle(int id)
    {
        Article? article = _repository.GetArticleById(id);
        if (article is null)
        {
            return NotFound();
        }

        _repository.RemoveArticle(article);

        article.SetUpdatedAtToNow();
        _repository.SaveChanges();

        _logger.LogInformation("DELETE Article : {id} -- {date}", article.Id, DateTime.UtcNow);

        _sender.RequestForPublish(RequestType.Deleted, article);

        return NoContent();
    }
    [HttpPatch("{id}")]
    public ActionResult<ArticleReadDto> UpdateArticle(int id, JsonPatchDocument<ArticleUpdateDto> patchDoc )
    {
        Article? article = _repository.GetArticleById(id);
        if (article is null)
        {
            return NotFound();
        }

        var articleToWrite = _mapper.Map<ArticleUpdateDto>(article);
        patchDoc.ApplyTo(articleToWrite);

        _mapper.Map(articleToWrite, article);

        if(!Validate(article))
        {
            return UnprocessableEntity(ModelState);
        }

        article.SetUpdatedAtToNow();
        _repository.SaveChanges();       

        _logger.LogInformation("PATCH : Article Updated : {id} -- {date}",  article.Id, DateTime.UtcNow);

        _sender.RequestForPublish(RequestType.Updated, article);

        ArticleReadDto readDto = _mapper.Map<ArticleReadDto>(article);
        return Ok(readDto);
    }

    private bool Validate(Article article)
    {
        ValidationResult result = _validator.Validate(article);
        if(result.IsValid)
        {
            return true;
        }
        foreach(ValidationFailure failure in result.Errors)
        {
            ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
        }
        return false;
    }
}
