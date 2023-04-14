using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.MessageBus;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Services
{
    public class ArticleService : IArticleService
    {
        private readonly ILogger<ArticleService> _logger;
        private readonly IArticleRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMessageBusSender<Article> _sender;
        private readonly IValidator<Article> _validator;

        public ArticleService(ILogger<ArticleService> logger,
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

        public Result<ArticleReadDto> CreateArticle(ArticleCreateDto dto)
        {
            Article article = _mapper.Map<Article>(dto);

            ValidationResult validationResult = _validator.Validate(article);

            if (!validationResult.IsValid)
            {
                return Result<ArticleReadDto>.Failure(validationResult.ToDictionary());
            }
       
             _repository.AddArticle(article);

            _repository.SaveChanges();

            _logger.LogInformation("Article Created : Id = {id}, Date = {date}", article.Id, DateTime.UtcNow);

            _sender.RequestForPublish(RequestType.Created, article, ChangeType.All);

            ArticleReadDto articleReadDto = _mapper.Map<ArticleReadDto>(article);

            return Result<ArticleReadDto>.Success(articleReadDto);
        }

        public Result<IEnumerable<ArticleReadDto>> GetAllArticles()
        {
            IEnumerable<Article> articles = _repository.GetAllArticles();
            IEnumerable<ArticleReadDto> articleDtos = _mapper.Map<IEnumerable<ArticleReadDto>>(articles);

            return Result<IEnumerable<ArticleReadDto>>.Success(articleDtos);
        }

        public Result<ArticleReadDto> GetArticleById(int articleId)
        {
            Article? article = _repository.GetArticleById(articleId);

            if (article is null)
            {
                return Result<ArticleReadDto>.Failure(GetNotFoundResult(articleId));
            }

            ArticleReadDto readDto = _mapper.Map<ArticleReadDto>(article);

            return Result<ArticleReadDto>.Success(readDto);
        }

        public Result<ArticleReadDto> PatchArticle(int articleId, JsonPatchDocument<ArticleUpdateDto> document)
        {
            Article? article = _repository.GetArticleById(articleId);

            if (article is null)
            {
                return Result<ArticleReadDto>.Failure(GetNotFoundResult(articleId));
            }

            //Catching exceptions due to malformed PATCH document ex. Wrong operation
            try
            {
                _repository.UpdateArticle(article, document);
            } 
            catch(Exception ex)
            {
                return Result<ArticleReadDto>.Failure(GetCaughtExceptionResult(ex.Message));
            }


            ValidationResult validationResult = _validator.Validate(article);

            if(!validationResult.IsValid)
            {
                return Result<ArticleReadDto>.Failure(validationResult.ToDictionary());
            }

            ChangeType changed = _repository.TrackChanges(article);

            _repository.SaveChanges();

             _logger.LogInformation("Article Updated : Id = {id}, Date = {date}", article.Id, DateTime.UtcNow);

            _sender.RequestForPublish(RequestType.Updated, article, changed);

            ArticleReadDto readDto = _mapper.Map<ArticleReadDto>(article);

            return Result<ArticleReadDto>.Success(readDto);
        }

        public Result RemoveArticleById(int articleId)
        {
            Article? article = _repository.GetArticleById(articleId);

            if (article is null)
            {
                return Result.Failure(GetNotFoundResult(articleId));
            }

            _repository.RemoveArticle(article);

            _repository.SaveChanges();

            _logger.LogInformation("Article Deleted : Id = {id}, Date = {date}", article.Id, DateTime.UtcNow);

            _sender.RequestForPublish(RequestType.Deleted, article, ChangeType.All);

            return Result.Success();
        }

        #region Private Methods

        private IDictionary<string, string[]> GetNotFoundResult(int articleId)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(Article)] = new string[] { $"Article not found : ID = {articleId}" }
            };
        }

        private IDictionary<string, string[]> GetCaughtExceptionResult(string message)
        {
            return new Dictionary<string, string[]>
            {
                ["message"] = new string[] { message }
            };
        }
        #endregion
    }
}
