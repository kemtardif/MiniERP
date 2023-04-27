using AutoMapper;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Handlers
{
    public class HandlerBase
    {
        protected readonly IRepository _repository;
        protected readonly IMapper _mapper;

        protected HandlerBase(IRepository repository,
                             IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        protected IDictionary<string, string[]> GetNotFoundResult(int id)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(Article)] = new string[] { $"Article not found : ID = {id}" }
            };
        }
    }
}
