using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Grpc;
using MiniERP.ArticleService.MessageBus;
using MiniERP.ArticleService.Models;
using System.Transactions;

namespace MiniERP.ArticleService.Services
{
    public class ArticleService : IArticleService
    {
        private readonly ILogger<ArticleService> _logger;
        private readonly IArticleRepository _repository;
        private readonly IMapper _mapper;
        private readonly IInventoryDataClient _dataClient;
        private readonly IValidator<Article> _validator;

        public ArticleService(ILogger<ArticleService> logger,
                            IArticleRepository repository,
                            IMapper mapper,
                            IInventoryDataClient dataClient,
                            IValidator<Article> validator)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _dataClient = dataClient;
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

            using var scope = new TransactionScope();

                _repository.SaveChanges();

                _dataClient.InventoryItemsCreated(new List<Article> { article });

                _logger.LogInformation("Article Created : Id = {id}, Date = {date}", article.Id, DateTime.UtcNow);

            scope.Complete();

          
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

            using var scope = new TransactionScope();

                _repository.SaveChanges();

                if(changed.HasFlag(ChangeType.Inventory))
                {
                    _dataClient.InventoryItemsUpdated(new List<Article>() { article });
                }

                _logger.LogInformation("Article Updated : Id = {id}, Date = {date}", article.Id, DateTime.UtcNow);

            scope.Complete();

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
