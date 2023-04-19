using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Data
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ArticleRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddArticle(Article item)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.Articles.Add(item);
        }

        public IEnumerable<Article> GetAllArticles()
        {
            return _context.Articles.ToList();
        }

        public Article? GetArticleById(int id)
        {
            return _context.Articles.FirstOrDefault(x => x.Id == id);
        }

        public void RemoveArticle(Article item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            item.Status = ArticleStatus.Closed;
        }

        public bool SaveChanges()
        {
            return  _context.SaveChanges() >= 0;
        }
        public Article UpdateArticle(Article item, JsonPatchDocument<ArticleUpdateDto> json)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if(json is null)
            {
                throw new ArgumentNullException(nameof(json));
            }
            var articleToWrite = _mapper.Map<ArticleUpdateDto>(item);
            json.ApplyTo(articleToWrite);

            _mapper.Map(articleToWrite, item);

            return item;
        }
    }
}
